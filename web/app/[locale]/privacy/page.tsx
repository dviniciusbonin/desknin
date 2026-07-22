import type { Metadata } from "next";
import { notFound } from "next/navigation";
import { LegalDocument } from "@/components/LegalDocument";
import { SiteFooter } from "@/components/SiteFooter";
import { SiteHeader } from "@/components/SiteHeader";
import { getDictionary } from "@/lib/dictionary";
import { isLocale, type Locale } from "@/lib/i18n";

type PrivacyPageProps = {
  params: Promise<{ locale: string }>;
};

export async function generateMetadata({ params }: PrivacyPageProps): Promise<Metadata> {
  const { locale: localeParam } = await params;
  if (!isLocale(localeParam)) {
    return {};
  }

  const dictionary = await getDictionary(localeParam);

  return {
    title: dictionary.privacy.title,
    description: dictionary.privacy.intro,
    alternates: {
      canonical: `/${localeParam}/privacy`,
    },
  };
}

export default async function PrivacyPage({ params }: PrivacyPageProps) {
  const { locale: localeParam } = await params;
  if (!isLocale(localeParam)) {
    notFound();
  }

  const locale = localeParam as Locale;
  const dictionary = await getDictionary(locale);

  return (
    <div className="flex min-h-dvh flex-col bg-surface">
      <SiteHeader locale={locale} dictionary={dictionary} />
      <main className="flex-1">
        <LegalDocument document={dictionary.privacy} />
      </main>
      <SiteFooter locale={locale} dictionary={dictionary} />
    </div>
  );
}
