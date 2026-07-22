"use server";

import { getDictionary } from "@/lib/dictionary";
import { defaultLocale, isLocale, type Locale } from "@/lib/i18n";

export type ForgotPasswordState = {
  ok: boolean;
  message?: string;
  fieldErrors?: Partial<Record<"email", string>>;
};

const emailPattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

export async function submitForgotPassword(
  _prev: ForgotPasswordState,
  formData: FormData,
): Promise<ForgotPasswordState> {
  const localeValue = String(formData.get("locale") ?? defaultLocale);
  const locale: Locale = isLocale(localeValue) ? localeValue : defaultLocale;
  const dictionary = await getDictionary(locale);
  const copy = dictionary.forgotPassword;

  const email = String(formData.get("email") ?? "").trim().toLowerCase();

  if (!emailPattern.test(email)) {
    return {
      ok: false,
      fieldErrors: { email: copy.errors.email },
      message: copy.errors.generic,
    };
  }

  const payload = { email, locale };

  const apiUrl = process.env.APP_API_URL?.replace(/\/$/, "");
  if (apiUrl) {
    try {
      const response = await fetch(`${apiUrl}/api/auth/forgot-password`, {
        method: "POST",
        headers: { "Content-Type": "application/json", Accept: "application/json" },
        body: JSON.stringify(payload),
        cache: "no-store",
      });

      // Always show the same success copy to avoid account enumeration when API agrees.
      if (response.ok || response.status === 404) {
        return { ok: true, message: copy.successMessage };
      }

      const detail = await response.text();
      return { ok: false, message: detail || copy.errors.api };
    } catch {
      return { ok: false, message: copy.errors.network };
    }
  }

  return { ok: true, message: copy.successMessage };
}
