import type { Dictionary } from "@/lib/dictionary";

type LegalDocumentProps = {
  document: Dictionary["privacy"] | Dictionary["terms"];
};

export function LegalDocument({ document }: LegalDocumentProps) {
  return (
    <article className="mx-auto w-full max-w-3xl px-5 py-12 sm:px-8 sm:py-16">
      <h1 className="font-display text-3xl font-semibold tracking-tight text-ink sm:text-4xl">
        {document.title}
      </h1>
      <p className="mt-2 font-sans text-sm text-ink-muted">{document.updated}</p>
      <p className="mt-6 font-sans text-base leading-relaxed text-ink-muted sm:text-lg">
        {document.intro}
      </p>
      <div className="mt-10 flex flex-col gap-8">
        {document.sections.map((section, index) => (
          <section key={section.heading} aria-labelledby={`legal-section-${index}`}>
            <h2
              id={`legal-section-${index}`}
              className="font-display text-xl font-semibold text-ink"
            >
              {section.heading}
            </h2>
            <p className="mt-2 font-sans text-base leading-relaxed text-ink-muted">
              {section.body}
            </p>
          </section>
        ))}
      </div>
    </article>
  );
}
