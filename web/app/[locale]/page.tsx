import type { Metadata } from "next";
import Link from "next/link";
import { notFound } from "next/navigation";
import { ContactSection } from "@/components/ContactSection";
import { FaqSection } from "@/components/FaqSection";
import { FeaturesSection } from "@/components/FeaturesSection";
import { FinalCtaSection } from "@/components/FinalCtaSection";
import { Logo } from "@/components/Logo";
import { PlansSection } from "@/components/PlansSection";
import { ProductPreview } from "@/components/ProductPreview";
import { SiteFooter } from "@/components/SiteFooter";
import { SiteHeader } from "@/components/SiteHeader";
import { getDictionary } from "@/lib/dictionary";
import { isLocale, localePath, type Locale } from "@/lib/i18n";
import { getSiteUrl, siteConfig, standardPricingByLocale } from "@/lib/site";

export const dynamic = "force-static";

type HomePageProps = {
  params: Promise<{ locale: string }>;
};

export async function generateMetadata({ params }: HomePageProps): Promise<Metadata> {
  const { locale: localeParam } = await params;
  if (!isLocale(localeParam)) {
    return {};
  }

  const dictionary = await getDictionary(localeParam);

  return {
    title: dictionary.meta.title,
    description: dictionary.meta.description,
    alternates: {
      canonical: `/${localeParam}`,
    },
  };
}

export default async function Home({ params }: HomePageProps) {
  const { locale: localeParam } = await params;
  if (!isLocale(localeParam)) {
    notFound();
  }

  const locale = localeParam as Locale;
  const dictionary = await getDictionary(locale);
  const siteUrl = getSiteUrl();

  const pricing = standardPricingByLocale[locale];

  const jsonLd = {
    "@context": "https://schema.org",
    "@graph": [
      {
        "@type": "WebSite",
        "@id": `${siteUrl}/#website`,
        url: `${siteUrl}/${locale}`,
        name: siteConfig.name,
        description: dictionary.meta.description,
        inLanguage: locale,
        publisher: {
          "@id": `${siteUrl}/#organization`,
        },
      },
      {
        "@type": "Organization",
        "@id": `${siteUrl}/#organization`,
        name: siteConfig.name,
        url: siteUrl,
        email: siteConfig.contactEmail,
        logo: {
          "@type": "ImageObject",
          url: `${siteUrl}/logo.svg`,
        },
        contactPoint: {
          "@type": "ContactPoint",
          email: siteConfig.contactEmail,
          contactType: "sales",
        },
      },
      {
        "@type": "SoftwareApplication",
        name: siteConfig.name,
        applicationCategory: "BusinessApplication",
        operatingSystem: "Web",
        description: dictionary.meta.description,
        url: `${siteUrl}/${locale}`,
        offers: [
          {
            "@type": "Offer",
            name: `${dictionary.plans.trial.name}`,
            description: dictionary.plans.trial.description,
            price: "0",
            priceCurrency: pricing.currency,
            url: `${siteUrl}${localePath(locale, "/onboarding?plan=trial")}`,
          },
          {
            "@type": "Offer",
            name: `${dictionary.plans.standard.name} (${dictionary.plans.billingMonthly})`,
            description: dictionary.plans.standard.description,
            price: String(pricing.monthly.amount),
            priceCurrency: pricing.currency,
            url: `${siteUrl}${localePath(locale, "/onboarding?plan=standard&billing=monthly")}`,
          },
          {
            "@type": "Offer",
            name: `${dictionary.plans.standard.name} (${dictionary.plans.billingAnnual})`,
            description: dictionary.plans.standard.description,
            price: String(pricing.annual.amount),
            priceCurrency: pricing.currency,
            url: `${siteUrl}${localePath(locale, "/onboarding?plan=standard&billing=annual")}`,
          },
        ],
      },
      {
        "@type": "FAQPage",
        mainEntity: dictionary.faq.items.map((item) => ({
          "@type": "Question",
          name: item.question,
          acceptedAnswer: {
            "@type": "Answer",
            text: item.answer,
          },
        })),
      },
    ],
  };

  return (
    <>
      <script
        type="application/ld+json"
        dangerouslySetInnerHTML={{ __html: JSON.stringify(jsonLd) }}
      />
      <div className="flex min-h-dvh flex-col">
        <div className="hero-atmosphere relative overflow-hidden">
          <SiteHeader locale={locale} dictionary={dictionary} />

          <section
            aria-labelledby="hero-heading"
            className="relative z-10 mx-auto grid w-full max-w-6xl items-center gap-6 px-5 pb-8 pt-3 sm:gap-10 sm:px-8 sm:pb-12 sm:pt-4 lg:min-h-[calc(100dvh-5.5rem)] lg:grid-cols-[minmax(0,1fr)_minmax(0,1.05fr)] lg:gap-12 lg:pb-16 lg:pt-8"
          >
            <div className="flex flex-col items-start">
              <Logo size="lg" className="animate-rise mb-3 sm:mb-8" />
              <h1
                id="hero-heading"
                className="animate-rise delay-1 font-display max-w-xl text-[1.65rem] font-semibold leading-tight tracking-tight text-ink sm:text-4xl md:text-[2.75rem] md:leading-[1.15]"
              >
                {dictionary.hero.heading}
              </h1>
              <div className="animate-rise delay-2 mt-5 flex w-full flex-col gap-3 sm:order-3 sm:mt-8 sm:w-auto sm:flex-row">
                <Link
                  href={localePath(locale, "/onboarding")}
                  className="inline-flex w-full items-center justify-center rounded-md bg-primary px-5 py-3 font-sans text-sm font-semibold text-white transition-colors hover:bg-primary-deep sm:w-auto"
                >
                  {dictionary.hero.cta}
                </Link>
                <Link
                  href={localePath(locale, "/login")}
                  className="inline-flex w-full items-center justify-center rounded-md border border-line bg-surface-elevated px-5 py-3 font-sans text-sm font-semibold text-ink transition-colors hover:border-primary hover:text-primary xl:hidden sm:w-auto"
                >
                  {dictionary.nav.signIn}
                </Link>
              </div>
              <p className="animate-rise delay-3 mt-4 max-w-md font-sans text-base leading-relaxed text-ink-muted sm:order-2 sm:mt-4 sm:text-lg">
                {dictionary.hero.supporting}
              </p>
            </div>

            <div className="animate-drift delay-2 -mx-5 h-[min(28vh,200px)] sm:-mx-8 sm:h-[min(42vh,320px)] lg:mx-0 lg:h-[min(70vh,560px)] lg:self-stretch">
              <ProductPreview />
            </div>
          </section>
        </div>

        <main>
          <FeaturesSection dictionary={dictionary} />
          <PlansSection locale={locale} dictionary={dictionary} />
          <FaqSection dictionary={dictionary} />
          <ContactSection dictionary={dictionary} />
          <FinalCtaSection locale={locale} dictionary={dictionary} />
        </main>

        <SiteFooter locale={locale} dictionary={dictionary} />
      </div>
    </>
  );
}
