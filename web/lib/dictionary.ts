import type { Locale } from "@/lib/i18n";
import { defaultLocale } from "@/lib/i18n";
import enUS from "@/messages/en-us.json";
import esES from "@/messages/es-es.json";
import ptBR from "@/messages/pt-br.json";

const dictionaries = {
  "en-us": enUS,
  "pt-br": ptBR,
  "es-es": esES,
} as const;

export type Dictionary = typeof enUS;

export async function getDictionary(locale: Locale): Promise<Dictionary> {
  return dictionaries[locale] ?? dictionaries[defaultLocale];
}

export function t(
  template: string,
  values: Record<string, string>,
): string {
  return Object.entries(values).reduce(
    (result, [key, value]) => result.replaceAll(`{${key}}`, value),
    template,
  );
}
