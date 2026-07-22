import type { Metadata } from "next";
import { notFound } from "next/navigation";
import { OnboardingForm } from "@/components/OnboardingForm";
import { SiteFooter } from "@/components/SiteFooter";
import { SiteHeader } from "@/components/SiteHeader";
import { getDictionary } from "@/lib/dictionary";
import { isLocale, type Locale } from "@/lib/i18n";
import { isBillingCycle, type BillingCycle } from "@/lib/site";

type OnboardingPageProps = {
  params: Promise<{ locale: string }>;
  searchParams: Promise<{ plan?: string; billing?: string }>;
};

export async function generateMetadata({
  params,
}: {
  params: Promise<{ locale: string }>;
}): Promise<Metadata> {
  const { locale: localeParam } = await params;
  if (!isLocale(localeParam)) {
    return {};
  }

  const dictionary = await getDictionary(localeParam);

  return {
    title: dictionary.meta.onboardingTitle,
    description: dictionary.meta.onboardingDescription,
    alternates: {
      canonical: `/${localeParam}/onboarding`,
    },
    robots: {
      index: true,
      follow: true,
    },
  };
}

export default async function OnboardingPage({ params, searchParams }: OnboardingPageProps) {
  const { locale: localeParam } = await params;
  if (!isLocale(localeParam)) {
    notFound();
  }

  const locale = localeParam as Locale;
  const dictionary = await getDictionary(locale);
  const query = await searchParams;
  const defaultPlan = query.plan === "standard" ? "standard" : "trial";
  const defaultBilling: BillingCycle =
    query.billing && isBillingCycle(query.billing) ? query.billing : "monthly";

  return (
    <div className="flex min-h-dvh flex-col bg-surface">
      <SiteHeader locale={locale} dictionary={dictionary} />
      <main className="mx-auto flex w-full max-w-2xl flex-1 flex-col px-5 py-12 sm:px-8 sm:py-16">
        <h1 className="font-display text-3xl font-semibold tracking-tight text-ink sm:text-4xl">
          {dictionary.onboarding.heading}
        </h1>
        <p className="mt-3 font-sans text-base leading-relaxed text-ink-muted sm:text-lg">
          {dictionary.onboarding.supporting}
        </p>
        <div className="mt-8">
          <OnboardingForm
            locale={locale}
            dictionary={dictionary.onboarding}
            defaultPlan={defaultPlan}
            defaultBilling={defaultBilling}
          />
        </div>
      </main>
      <SiteFooter locale={locale} dictionary={dictionary} />
    </div>
  );
}
