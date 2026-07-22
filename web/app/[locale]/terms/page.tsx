import type { Metadata } from "next";
import { notFound } from "next/navigation";
import { LegalDocument } from "@/components/LegalDocument";
import { SiteFooter } from "@/components/SiteFooter";
import { SiteHeader } from "@/components/SiteHeader";
import { getDictionary } from "@/lib/dictionary";
import { isLocale, type Locale } from "@/lib/i18n";

type TermsPageProps = {
  params: Promise<{ locale: string }>;
};

export async function generateMetadata({ params }: TermsPageProps): Promise<Metadata> {
  const { locale: localeParam } = await params;
  if (!isLocale(localeParam)) {
    return {};
  }

  const dictionary = await getDictionary(localeParam);

  return {
    title: dictionary.terms.title,
    description: dictionary.terms.intro,
    alternates: {
      canonical: `/${localeParam}/terms`,
    },
  };
}

export default async function TermsPage({ params }: TermsPageProps) {
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
        <LegalDocument document={dictionary.terms} />
      </main>
      <SiteFooter locale={locale} dictionary={dictionary} />
    </div>
  );
}
