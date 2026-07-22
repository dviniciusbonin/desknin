import Link from "next/link";
import { LocaleSwitcher } from "@/components/LocaleSwitcher";
import { Logo } from "@/components/Logo";
import { MobileNav } from "@/components/MobileNav";
import { ThemeToggle } from "@/components/ThemeToggle";
import type { Dictionary } from "@/lib/dictionary";
import { localePath, type Locale } from "@/lib/i18n";

type SiteHeaderProps = {
  locale: Locale;
  dictionary: Dictionary;
};

export function SiteHeader({ locale, dictionary }: SiteHeaderProps) {
  const home = localePath(locale);

  return (
    <header className="animate-fade relative z-10 border-b border-line/60 bg-surface/70 backdrop-blur-sm">
      <div className="mx-auto flex w-full max-w-6xl items-center justify-between gap-2 px-4 py-3.5 sm:gap-4 sm:px-8 sm:py-5">
        <Link href={home} aria-label={dictionary.nav.home} className="min-w-0 shrink">
          <Logo />
        </Link>

        <nav
          aria-label="Primary"
          className="flex shrink-0 items-center justify-end gap-1.5 sm:gap-3"
        >
          <div className="hidden items-center gap-5 xl:flex">
            <a
              href={`${home}#features`}
              className="font-sans text-sm font-medium text-ink-muted transition-colors hover:text-primary"
            >
              {dictionary.nav.features}
            </a>
            <a
              href={`${home}#plans`}
              className="font-sans text-sm font-medium text-ink-muted transition-colors hover:text-primary"
            >
              {dictionary.nav.plans}
            </a>
            <a
              href={`${home}#faq`}
              className="font-sans text-sm font-medium text-ink-muted transition-colors hover:text-primary"
            >
              {dictionary.nav.faq}
            </a>
            <a
              href={`${home}#contact`}
              className="font-sans text-sm font-medium text-ink-muted transition-colors hover:text-primary"
            >
              {dictionary.nav.contact}
            </a>
          </div>

          <LocaleSwitcher label={dictionary.locale.label} currentLocale={locale} />
          <ThemeToggle
            label={dictionary.theme.label}
            lightLabel={dictionary.theme.light}
            darkLabel={dictionary.theme.dark}
          />

          <Link
            href={localePath(locale, "/login")}
            className="hidden font-sans text-sm font-medium text-ink-muted transition-colors hover:text-primary xl:inline"
          >
            {dictionary.nav.signIn}
          </Link>

          <Link
            href={localePath(locale, "/onboarding")}
            className="hidden items-center justify-center rounded-md bg-primary px-3.5 py-2 font-sans text-sm font-semibold text-white transition-colors hover:bg-primary-deep xl:inline-flex"
          >
            {dictionary.nav.getStarted}
          </Link>

          <MobileNav locale={locale} dictionary={dictionary} />
        </nav>
      </div>
    </header>
  );
}
