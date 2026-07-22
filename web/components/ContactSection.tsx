import type { Dictionary } from "@/lib/dictionary";
import { getWhatsAppUrl } from "@/lib/site";

type ContactSectionProps = {
  dictionary: Dictionary;
};

export function ContactSection({ dictionary }: ContactSectionProps) {
  const whatsappUrl = getWhatsAppUrl(dictionary.contact.whatsappMessage);

  return (
    <section
      id="contact"
      aria-labelledby="contact-heading"
      className="scroll-mt-24 border-t border-line bg-surface"
    >
      <div className="mx-auto grid max-w-6xl gap-8 px-5 py-16 sm:px-8 sm:py-20 lg:grid-cols-[1.2fr_0.8fr] lg:items-end">
        <div>
          <h2
            id="contact-heading"
            className="font-display text-3xl font-semibold tracking-tight text-ink sm:text-4xl"
          >
            {dictionary.contact.heading}
          </h2>
          <p className="mt-3 max-w-xl font-sans text-base leading-relaxed text-ink-muted sm:text-lg">
            {dictionary.contact.supporting}
          </p>
        </div>
        <div>
          <a
            href={whatsappUrl}
            target="_blank"
            rel="noopener noreferrer"
            className="inline-flex w-full items-center justify-center rounded-md bg-primary px-5 py-3 font-sans text-sm font-semibold text-white transition-colors hover:bg-primary-deep sm:w-auto"
          >
            {dictionary.contact.cta}
          </a>
        </div>
      </div>
    </section>
  );
}
