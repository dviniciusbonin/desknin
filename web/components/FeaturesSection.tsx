import type { Dictionary } from "@/lib/dictionary";

type FeaturesSectionProps = {
  dictionary: Dictionary;
};

export function FeaturesSection({ dictionary }: FeaturesSectionProps) {
  return (
    <section
      id="features"
      aria-labelledby="features-heading"
      className="scroll-mt-24 border-t border-line bg-surface"
    >
      <div className="mx-auto max-w-6xl px-5 py-16 sm:px-8 sm:py-20">
        <div className="max-w-2xl">
          <h2
            id="features-heading"
            className="font-display text-3xl font-semibold tracking-tight text-ink sm:text-4xl"
          >
            {dictionary.features.heading}
          </h2>
          <p className="mt-3 font-sans text-base leading-relaxed text-ink-muted sm:text-lg">
            {dictionary.features.supporting}
          </p>
        </div>

        <ul className="mt-12 grid list-none gap-10 p-0 md:grid-cols-2">
          {dictionary.features.items.map((item) => (
            <li key={item.title} className="border-t border-line pt-6">
              <h3 className="font-display text-xl font-semibold text-ink">{item.title}</h3>
              <p className="mt-2 font-sans text-sm leading-relaxed text-ink-muted sm:text-base">
                {item.description}
              </p>
            </li>
          ))}
        </ul>
      </div>
    </section>
  );
}
