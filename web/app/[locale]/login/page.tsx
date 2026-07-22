import type { Metadata } from "next";
import { notFound } from "next/navigation";
import { LoginForm } from "@/components/LoginForm";
import { Logo } from "@/components/Logo";
import { SiteFooter } from "@/components/SiteFooter";
import { SiteHeader } from "@/components/SiteHeader";
import { getDictionary } from "@/lib/dictionary";
import { isLocale, type Locale } from "@/lib/i18n";

type LoginPageProps = {
  params: Promise<{ locale: string }>;
};

export async function generateMetadata({ params }: LoginPageProps): Promise<Metadata> {
  const { locale: localeParam } = await params;
  if (!isLocale(localeParam)) {
    return {};
  }

  const dictionary = await getDictionary(localeParam);

  return {
    title: dictionary.meta.loginTitle,
    description: dictionary.meta.loginDescription,
    alternates: {
      canonical: `/${localeParam}/login`,
    },
    robots: {
      index: false,
      follow: true,
    },
  };
}

export default async function LoginPage({ params }: LoginPageProps) {
  const { locale: localeParam } = await params;
  if (!isLocale(localeParam)) {
    notFound();
  }

  const locale = localeParam as Locale;
  const dictionary = await getDictionary(locale);

  return (
    <div className="flex min-h-dvh flex-col bg-surface">
      <SiteHeader locale={locale} dictionary={dictionary} />
      <main className="mx-auto flex w-full max-w-md flex-1 flex-col justify-center px-5 py-12 sm:px-8 sm:py-16">
        <Logo className="mb-8" />
        <h1 className="font-display text-3xl font-semibold tracking-tight text-ink sm:text-4xl">
          {dictionary.login.heading}
        </h1>
        <p className="mt-3 font-sans text-base leading-relaxed text-ink-muted sm:text-lg">
          {dictionary.login.supporting}
        </p>
        <div className="mt-8">
          <LoginForm locale={locale} dictionary={dictionary.login} />
        </div>
      </main>
      <SiteFooter locale={locale} dictionary={dictionary} />
    </div>
  );
}
