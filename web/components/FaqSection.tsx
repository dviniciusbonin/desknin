import type { Dictionary } from "@/lib/dictionary";

type FaqSectionProps = {
  dictionary: Dictionary;
};

export function FaqSection({ dictionary }: FaqSectionProps) {
  return (
    <section
      id="faq"
      aria-labelledby="faq-heading"
      className="scroll-mt-24 border-t border-line bg-surface"
    >
      <div className="mx-auto max-w-6xl px-5 py-16 sm:px-8 sm:py-20">
        <div className="max-w-2xl">
          <h2
            id="faq-heading"
            className="font-display text-3xl font-semibold tracking-tight text-ink sm:text-4xl"
          >
            {dictionary.faq.heading}
          </h2>
          <p className="mt-3 font-sans text-base leading-relaxed text-ink-muted sm:text-lg">
            {dictionary.faq.supporting}
          </p>
        </div>

        <div className="mt-10 max-w-3xl divide-y divide-line border-y border-line">
          {dictionary.faq.items.map((item) => (
            <details key={item.question} className="group py-4">
              <summary className="cursor-pointer list-none font-display text-base font-semibold text-ink marker:content-none sm:text-lg [&::-webkit-details-marker]:hidden">
                <span className="flex items-start justify-between gap-3 sm:gap-4">
                  <span className="min-w-0 text-left">{item.question}</span>
                  <span
                    aria-hidden
                    className="mt-0.5 shrink-0 font-sans text-ink-muted transition-transform group-open:rotate-45"
                  >
                    +
                  </span>
                </span>
              </summary>
              <p className="mt-3 max-w-2xl font-sans text-sm leading-relaxed text-ink-muted sm:text-base">
                {item.answer}
              </p>
            </details>
          ))}
        </div>
      </div>
    </section>
  );
}
