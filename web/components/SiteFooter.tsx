import Link from "next/link";
import { Logo } from "@/components/Logo";
import type { Dictionary } from "@/lib/dictionary";
import { localePath, type Locale } from "@/lib/i18n";
import { siteConfig } from "@/lib/site";

type SiteFooterProps = {
  locale: Locale;
  dictionary: Dictionary;
};

export function SiteFooter({ locale, dictionary }: SiteFooterProps) {
  const home = localePath(locale);

  return (
    <footer className="border-t border-line bg-footer-bg text-white">
      <div className="mx-auto flex w-full max-w-6xl flex-col gap-8 px-5 py-10 pb-[max(2.5rem,env(safe-area-inset-bottom))] sm:px-8">
        <div className="flex flex-col gap-6 sm:flex-row sm:items-start sm:justify-between">
          <div className="flex flex-col gap-3">
            <Logo tone="onDark" />
            <p className="max-w-sm font-sans text-sm text-footer-muted">
              © {new Date().getFullYear()} {siteConfig.name}. {dictionary.footer.tagline}
            </p>
          </div>

          <nav aria-label={dictionary.footer.legal} className="flex flex-wrap gap-x-6 gap-y-2">
            <a
              href={`${home}#features`}
              className="font-sans text-sm text-footer-muted transition-colors hover:text-white"
            >
              {dictionary.nav.features}
            </a>
            <a
              href={`${home}#plans`}
              className="font-sans text-sm text-footer-muted transition-colors hover:text-white"
            >
              {dictionary.nav.plans}
            </a>
            <a
              href={`${home}#faq`}
              className="font-sans text-sm text-footer-muted transition-colors hover:text-white"
            >
              {dictionary.nav.faq}
            </a>
            <a
              href={`${home}#contact`}
              className="font-sans text-sm text-footer-muted transition-colors hover:text-white"
            >
              {dictionary.nav.contact}
            </a>
            <Link
              href={localePath(locale, "/privacy")}
              className="font-sans text-sm text-footer-muted transition-colors hover:text-white"
            >
              {dictionary.nav.privacy}
            </Link>
            <Link
              href={localePath(locale, "/terms")}
              className="font-sans text-sm text-footer-muted transition-colors hover:text-white"
            >
              {dictionary.nav.terms}
            </Link>
          </nav>
        </div>
      </div>
    </footer>
  );
}
