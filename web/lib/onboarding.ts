export const countryCodes = [
  "AR",
  "AU",
  "AT",
  "BE",
  "BO",
  "BR",
  "CA",
  "CL",
  "CN",
  "CO",
  "CR",
  "CZ",
  "DE",
  "DK",
  "EC",
  "ES",
  "FI",
  "FR",
  "GB",
  "IE",
  "IN",
  "IT",
  "JP",
  "MX",
  "NL",
  "NO",
  "NZ",
  "PE",
  "PL",
  "PT",
  "PY",
  "SE",
  "SG",
  "US",
  "UY",
  "VE",
] as const;

export type CountryCode = (typeof countryCodes)[number];

export const timezones = [
  "America/Sao_Paulo",
  "America/Argentina/Buenos_Aires",
  "America/Santiago",
  "America/Bogota",
  "America/Lima",
  "America/Mexico_City",
  "America/New_York",
  "America/Chicago",
  "America/Denver",
  "America/Los_Angeles",
  "America/Toronto",
  "Europe/Lisbon",
  "Europe/Madrid",
  "Europe/London",
  "Europe/Paris",
  "Europe/Berlin",
  "Europe/Rome",
  "Europe/Amsterdam",
  "Asia/Tokyo",
  "Asia/Singapore",
  "Asia/Kolkata",
  "Australia/Sydney",
  "UTC",
] as const;

export type Timezone = (typeof timezones)[number];

export const companySizes = ["1-10", "11-50", "51-200", "201-1000", "1000+"] as const;

export type CompanySize = (typeof companySizes)[number];

export function isCountryCode(value: string): value is CountryCode {
  return countryCodes.includes(value as CountryCode);
}

export function isTimezone(value: string): value is Timezone {
  return timezones.includes(value as Timezone);
}

export function isCompanySize(value: string): value is CompanySize {
  return companySizes.includes(value as CompanySize);
}

export function getCountryLabel(locale: string, code: string): string {
  try {
    return new Intl.DisplayNames([locale], { type: "region" }).of(code) ?? code;
  } catch {
    return code;
  }
}

export function getTimezoneLabel(locale: string, zone: string): string {
  if (zone === "UTC") {
    return "UTC";
  }

  try {
    const parts = new Intl.DateTimeFormat(locale, {
      timeZone: zone,
      timeZoneName: "shortOffset",
    }).formatToParts(new Date());
    const offset = parts.find((part) => part.type === "timeZoneName")?.value ?? "";
    const city = zone.split("/").pop()?.replaceAll("_", " ") ?? zone;
    return offset ? `${city} (${offset})` : city;
  } catch {
    return zone;
  }
}
