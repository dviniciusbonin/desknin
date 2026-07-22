import Link from "next/link";
import type { Dictionary } from "@/lib/dictionary";
import { localePath, type Locale } from "@/lib/i18n";

type FinalCtaSectionProps = {
  locale: Locale;
  dictionary: Dictionary;
};

export function FinalCtaSection({ locale, dictionary }: FinalCtaSectionProps) {
  return (
    <section
      aria-labelledby="final-cta-heading"
      className="border-t border-line bg-primary"
    >
      <div className="mx-auto flex max-w-6xl flex-col gap-6 px-5 py-14 sm:flex-row sm:items-end sm:justify-between sm:px-8 sm:py-16">
        <div className="max-w-xl">
          <h2
            id="final-cta-heading"
            className="font-display text-3xl font-semibold tracking-tight text-white sm:text-4xl"
          >
            {dictionary.finalCta.heading}
          </h2>
          <p className="mt-3 font-sans text-base leading-relaxed text-white/85 sm:text-lg">
            {dictionary.finalCta.supporting}
          </p>
        </div>
        <Link
          href={localePath(locale, "/onboarding?plan=trial")}
          className="inline-flex w-full shrink-0 items-center justify-center rounded-md bg-white px-5 py-3 font-sans text-sm font-semibold text-primary transition-colors hover:bg-white/90 sm:w-auto"
        >
          {dictionary.finalCta.cta}
        </Link>
      </div>
    </section>
  );
}
