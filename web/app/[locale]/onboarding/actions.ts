"use server";

import { getDictionary, t } from "@/lib/dictionary";
import { defaultLocale, isLocale, locales, type Locale } from "@/lib/i18n";
import {
  isCompanySize,
  isCountryCode,
  isTimezone,
} from "@/lib/onboarding";
import { isBillingCycle } from "@/lib/site";

export type OnboardingField =
  | "companyName"
  | "website"
  | "companySize"
  | "adminName"
  | "adminEmail"
  | "adminPhone"
  | "country"
  | "timezone"
  | "supportLanguage"
  | "plan"
  | "billingCycle";

export type OnboardingState = {
  ok: boolean;
  message?: string;
  fieldErrors?: Partial<Record<OnboardingField, string>>;
};

const emailPattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
const phonePattern = /^\+?[\d\s().-]{7,20}$/;
const websitePattern = /^https?:\/\/[^\s/$.?#].[^\s]*$/i;

function normalizeWebsite(value: string): string {
  if (!value) {
    return "";
  }
  if (/^https?:\/\//i.test(value)) {
    return value;
  }
  return `https://${value}`;
}

export async function submitOnboarding(
  _prev: OnboardingState,
  formData: FormData,
): Promise<OnboardingState> {
  const localeValue = String(formData.get("locale") ?? defaultLocale);
  const locale: Locale = isLocale(localeValue) ? localeValue : defaultLocale;
  const dictionary = await getDictionary(locale);
  const copy = dictionary.onboarding;

  const companyName = String(formData.get("companyName") ?? "").trim();
  const websiteRaw = String(formData.get("website") ?? "").trim();
  const website = websiteRaw ? normalizeWebsite(websiteRaw) : "";
  const companySize = String(formData.get("companySize") ?? "").trim();
  const adminName = String(formData.get("adminName") ?? "").trim();
  const adminEmail = String(formData.get("adminEmail") ?? "").trim().toLowerCase();
  const adminPhone = String(formData.get("adminPhone") ?? "").trim();
  const country = String(formData.get("country") ?? "").trim().toUpperCase();
  const timezone = String(formData.get("timezone") ?? "").trim();
  const supportLanguage = String(formData.get("supportLanguage") ?? "").trim();
  const plan = String(formData.get("plan") ?? "").trim();
  const billingCycle = String(formData.get("billingCycle") ?? "").trim();

  const fieldErrors: OnboardingState["fieldErrors"] = {};

  if (companyName.length < 2) {
    fieldErrors.companyName = copy.errors.companyName;
  }
  if (website && !websitePattern.test(website)) {
    fieldErrors.website = copy.errors.website;
  }
  if (!isCompanySize(companySize)) {
    fieldErrors.companySize = copy.errors.companySize;
  }
  if (adminName.length < 2) {
    fieldErrors.adminName = copy.errors.adminName;
  }
  if (!emailPattern.test(adminEmail)) {
    fieldErrors.adminEmail = copy.errors.adminEmail;
  }
  if (adminPhone && !phonePattern.test(adminPhone)) {
    fieldErrors.adminPhone = copy.errors.adminPhone;
  }
  if (!isCountryCode(country)) {
    fieldErrors.country = copy.errors.country;
  }
  if (!isTimezone(timezone)) {
    fieldErrors.timezone = copy.errors.timezone;
  }
  if (!isLocale(supportLanguage)) {
    fieldErrors.supportLanguage = copy.errors.supportLanguage;
  }
  if (plan !== "trial" && plan !== "standard") {
    fieldErrors.plan = copy.errors.plan;
  }
  if (plan === "standard" && !isBillingCycle(billingCycle)) {
    fieldErrors.billingCycle = copy.errors.billingCycle;
  }

  if (Object.keys(fieldErrors).length > 0) {
    return { ok: false, fieldErrors, message: copy.errors.generic };
  }

  const payload = {
    companyName,
    website: website || null,
    companySize,
    adminName,
    adminEmail,
    adminPhone: adminPhone || null,
    country,
    timezone,
    supportLanguage,
    plan,
    billingCycle: plan === "standard" ? billingCycle : null,
    locale,
    availableLocales: [...locales],
  };

  const apiUrl = process.env.APP_API_URL?.replace(/\/$/, "");
  if (apiUrl) {
    try {
      const response = await fetch(`${apiUrl}/api/onboarding`, {
        method: "POST",
        headers: { "Content-Type": "application/json", Accept: "application/json" },
        body: JSON.stringify(payload),
        cache: "no-store",
      });

      if (!response.ok) {
        const detail = await response.text();
        return {
          ok: false,
          message: detail || copy.errors.api,
        };
      }
    } catch {
      return {
        ok: false,
        message: copy.errors.network,
      };
    }
  }

  return {
    ok: true,
    message: t(copy.successMessage, { company: companyName, email: adminEmail }),
  };
}
