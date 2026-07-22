import type { Metadata } from "next";
import { notFound } from "next/navigation";
import { BackToTop } from "@/components/BackToTop";
import { HtmlLang } from "@/components/HtmlLang";
import { getDictionary } from "@/lib/dictionary";
import { isLocale, locales, toHtmlLang, toOpenGraphLocale, type Locale } from "@/lib/i18n";
import { getSiteUrl, siteConfig } from "@/lib/site";

type LocaleLayoutProps = {
  children: React.ReactNode;
  params: Promise<{ locale: string }>;
};

export function generateStaticParams() {
  return locales.map((locale) => ({ locale }));
}

export async function generateMetadata({
  params,
}: {
  params: Promise<{ locale: string }>;
}): Promise<Metadata> {
  const { locale: localeParam } = await params;
  if (!isLocale(localeParam)) {
    return {};
  }

  const locale = localeParam;
  const dictionary = await getDictionary(locale);
  const siteUrl = getSiteUrl();

  return {
    metadataBase: new URL(siteUrl),
    title: {
      default: dictionary.meta.title,
      template: `%s · ${siteConfig.name}`,
    },
    description: dictionary.meta.description,
    applicationName: siteConfig.name,
    authors: [{ name: siteConfig.name }],
    creator: siteConfig.name,
    keywords: [
      "DeskNin",
      "help desk",
      "ticket management",
      "support tickets",
      "customer support",
    ],
    alternates: {
      canonical: `/${locale}`,
      languages: Object.fromEntries(locales.map((item) => [item, `/${item}`])),
    },
    openGraph: {
      type: "website",
      locale: toOpenGraphLocale(locale),
      url: `/${locale}`,
      siteName: siteConfig.name,
      title: dictionary.meta.title,
      description: dictionary.meta.description,
      images: [
        {
          url: "/logo.svg",
          width: 512,
          height: 512,
          alt: `${siteConfig.name} logo`,
        },
      ],
    },
    twitter: {
      card: "summary",
      title: dictionary.meta.title,
      description: dictionary.meta.description,
      images: ["/logo.svg"],
    },
    robots: {
      index: true,
      follow: true,
      googleBot: {
        index: true,
        follow: true,
        "max-image-preview": "large",
        "max-snippet": -1,
        "max-video-preview": -1,
      },
    },
    icons: {
      icon: [{ url: "/favicon.svg", type: "image/svg+xml" }],
    },
  };
}

export default async function LocaleLayout({ children, params }: LocaleLayoutProps) {
  const { locale: localeParam } = await params;
  if (!isLocale(localeParam)) {
    notFound();
  }

  const locale = localeParam as Locale;
  const dictionary = await getDictionary(locale);

  return (
    <>
      <HtmlLang lang={toHtmlLang(locale)} />
      {children}
      <BackToTop label={dictionary.backToTop} />
    </>
  );
}
