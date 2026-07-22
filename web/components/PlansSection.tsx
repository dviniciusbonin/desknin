"use client";

import Link from "next/link";
import { useState } from "react";
import type { Dictionary } from "@/lib/dictionary";
import { localePath, type Locale } from "@/lib/i18n";
import { planIds, planMeta, getWhatsAppUrl, type BillingCycle } from "@/lib/site";

type PlansSectionProps = {
  locale: Locale;
  dictionary: Dictionary;
};

export function PlansSection({ locale, dictionary }: PlansSectionProps) {
  const [billing, setBilling] = useState<BillingCycle>("monthly");
  const negotiateWhatsApp = getWhatsAppUrl(dictionary.contact.negotiateMessage);

  return (
    <section
      id="plans"
      aria-labelledby="plans-heading"
      className="scroll-mt-24 border-t border-line bg-surface-elevated"
    >
      <div className="mx-auto max-w-6xl px-5 py-16 sm:px-8 sm:py-20">
        <div className="flex max-w-2xl flex-col gap-6">
          <div>
            <h2
              id="plans-heading"
              className="font-display text-3xl font-semibold tracking-tight text-ink sm:text-4xl"
            >
              {dictionary.plans.heading}
            </h2>
            <p className="mt-3 font-sans text-base leading-relaxed text-ink-muted sm:text-lg">
              {dictionary.plans.supporting}
            </p>
          </div>

          <div
            role="group"
            aria-label={dictionary.plans.billingLabel}
            className="inline-flex max-w-full flex-wrap items-center gap-0.5 rounded-md border border-line bg-surface p-0.5"
          >
            <button
              type="button"
              onClick={() => setBilling("monthly")}
              aria-pressed={billing === "monthly"}
              className={`rounded-sm px-3 py-1.5 font-sans text-xs font-semibold transition-colors ${
                billing === "monthly"
                  ? "bg-primary text-white"
                  : "text-ink-muted hover:text-ink"
              }`}
            >
              {dictionary.plans.billingMonthly}
            </button>
            <button
              type="button"
              onClick={() => setBilling("annual")}
              aria-pressed={billing === "annual"}
              className={`inline-flex items-center gap-2 rounded-sm px-3 py-1.5 font-sans text-xs font-semibold transition-colors ${
                billing === "annual"
                  ? "bg-primary text-white"
                  : "text-ink-muted hover:text-ink"
              }`}
            >
              {dictionary.plans.billingAnnual}
              <span
                className={`rounded-sm px-1.5 py-0.5 text-[10px] font-bold tracking-wide ${
                  billing === "annual"
                    ? "bg-white/20 text-white"
                    : "bg-primary/10 text-primary"
                }`}
              >
                {dictionary.plans.annualSave}
              </span>
            </button>
          </div>
        </div>

        <ul className="mt-12 grid list-none gap-8 p-0 lg:grid-cols-3 lg:gap-10">
          {planIds.map((id) => {
            const plan = dictionary.plans[id];
            const meta = planMeta[id];
            const isStandard = id === "standard";
            const standard = dictionary.plans.standard;
            const price =
              isStandard && billing === "annual" ? standard.annualPrice : plan.price;
            const period =
              isStandard && billing === "annual" ? standard.annualPeriod : plan.period;
            const ctaClass = `mt-8 inline-flex w-full items-center justify-center rounded-md px-4 py-2.5 font-sans text-sm font-semibold transition-colors lg:w-fit ${
              meta.highlighted
                ? "bg-primary text-white hover:bg-primary-deep"
                : "border border-line bg-surface text-ink hover:border-primary"
            }`;

            return (
              <li
                key={id}
                className={`flex flex-col border-t-2 pt-6 ${
                  meta.highlighted ? "border-primary" : "border-line"
                }`}
              >
                <h3 className="font-display text-xl font-semibold text-ink">{plan.name}</h3>
                <p className="mt-2 font-sans text-sm leading-relaxed text-ink-muted">
                  {plan.description}
                </p>
                <p className="mt-6 font-display text-3xl font-semibold text-ink">
                  {price}
                  <span className="ml-2 font-sans text-sm font-medium text-ink-muted">
                    {period}
                  </span>
                </p>
                {isStandard && billing === "annual" ? (
                  <p className="mt-1 font-sans text-xs text-ink-muted">{standard.annualHint}</p>
                ) : null}
                <ul className="mt-6 flex flex-1 flex-col gap-2 font-sans text-sm text-ink-muted">
                  {plan.features.map((feature) => (
                    <li key={feature}>{feature}</li>
                  ))}
                </ul>
                {meta.ctaKind === "contact" ? (
                  <a
                    href={negotiateWhatsApp}
                    target="_blank"
                    rel="noopener noreferrer"
                    className={ctaClass}
                  >
                    {plan.cta}
                  </a>
                ) : (
                  <Link
                    href={localePath(
                      locale,
                      id === "standard"
                        ? `/onboarding?plan=standard&billing=${billing}`
                        : "/onboarding?plan=trial",
                    )}
                    className={ctaClass}
                  >
                    {plan.cta}
                  </Link>
                )}
              </li>
            );
          })}
        </ul>
      </div>
    </section>
  );
}
