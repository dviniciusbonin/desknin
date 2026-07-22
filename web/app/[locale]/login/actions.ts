"use server";

import { getDictionary } from "@/lib/dictionary";
import { defaultLocale, isLocale, type Locale } from "@/lib/i18n";

export type LoginField = "email" | "password";

export type LoginState = {
  ok: boolean;
  message?: string;
  redirectTo?: string;
  fieldErrors?: Partial<Record<LoginField, string>>;
};

const emailPattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

export async function submitLogin(
  _prev: LoginState,
  formData: FormData,
): Promise<LoginState> {
  const localeValue = String(formData.get("locale") ?? defaultLocale);
  const locale: Locale = isLocale(localeValue) ? localeValue : defaultLocale;
  const dictionary = await getDictionary(locale);
  const copy = dictionary.login;

  const email = String(formData.get("email") ?? "").trim().toLowerCase();
  const password = String(formData.get("password") ?? "");
  const rememberMe = formData.get("rememberMe") === "on";

  const fieldErrors: LoginState["fieldErrors"] = {};

  if (!emailPattern.test(email)) {
    fieldErrors.email = copy.errors.email;
  }
  if (password.length < 1) {
    fieldErrors.password = copy.errors.password;
  }

  if (Object.keys(fieldErrors).length > 0) {
    return { ok: false, fieldErrors, message: copy.errors.generic };
  }

  const payload = { email, password, rememberMe, locale };

  const apiUrl = process.env.APP_API_URL?.replace(/\/$/, "");
  if (apiUrl) {
    try {
      const response = await fetch(`${apiUrl}/api/auth/login`, {
        method: "POST",
        headers: { "Content-Type": "application/json", Accept: "application/json" },
        body: JSON.stringify(payload),
        cache: "no-store",
      });

      if (response.status === 401) {
        return { ok: false, message: copy.errors.invalid };
      }
      if (response.status === 423) {
        return { ok: false, message: copy.errors.locked };
      }
      if (!response.ok) {
        const detail = await response.text();
        return { ok: false, message: detail || copy.errors.api };
      }

      let redirectTo: string | undefined;
      try {
        const data = (await response.json()) as { redirectTo?: string };
        redirectTo = data.redirectTo;
      } catch {
        redirectTo = undefined;
      }

      return {
        ok: true,
        message: copy.successMessage,
        redirectTo,
      };
    } catch {
      return { ok: false, message: copy.errors.network };
    }
  }

  return {
    ok: true,
    message: copy.successMessage,
  };
}
