export const locales = ["en-us", "pt-br", "es-es"] as const;

export type Locale = (typeof locales)[number];

export const defaultLocale: Locale = "en-us";

export const localeLabels: Record<Locale, string> = {
  "en-us": "English",
  "pt-br": "Português",
  "es-es": "Español",
};

export const localeShortLabels: Record<Locale, string> = {
  "en-us": "EN",
  "pt-br": "PT",
  "es-es": "ES",
};

export function isLocale(value: string): value is Locale {
  return locales.includes(value as Locale);
}

export function localePath(locale: Locale, path = "/"): string {
  const normalized = path.startsWith("/") ? path : `/${path}`;
  if (normalized === "/") {
    return `/${locale}`;
  }
  return `/${locale}${normalized}`;
}

/** Open Graph locale tag, e.g. en_US */
export function toOpenGraphLocale(locale: Locale): string {
  const [language, region] = locale.split("-");
  return `${language}_${region.toUpperCase()}`;
}

/** BCP 47 tag for html lang, e.g. en-US */
export function toHtmlLang(locale: Locale): string {
  const [language, region] = locale.split("-");
  return `${language}-${region.toUpperCase()}`;
}
