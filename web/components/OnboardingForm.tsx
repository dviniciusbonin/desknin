"use client";

import { useActionState, useMemo, useState } from "react";
import Link from "next/link";
import {
  submitOnboarding,
  type OnboardingState,
} from "@/app/[locale]/onboarding/actions";
import type { Dictionary } from "@/lib/dictionary";
import { localeLabels, localePath, locales, type Locale } from "@/lib/i18n";
import {
  companySizes,
  countryCodes,
  getCountryLabel,
  getTimezoneLabel,
  timezones,
  type CompanySize,
} from "@/lib/onboarding";
import type { BillingCycle } from "@/lib/site";

const initialState: OnboardingState = { ok: false };

const fieldClassName =
  "mt-1.5 w-full rounded-md border border-line bg-surface-elevated px-3 py-2.5 font-sans text-sm text-ink outline-none transition-colors focus:border-primary";

const localeDefaults: Record<
  Locale,
  { country: string; timezone: string }
> = {
  "en-us": { country: "US", timezone: "America/New_York" },
  "pt-br": { country: "BR", timezone: "America/Sao_Paulo" },
  "es-es": { country: "ES", timezone: "Europe/Madrid" },
};

type OnboardingFormProps = {
  locale: Locale;
  dictionary: Dictionary["onboarding"];
  defaultPlan?: "trial" | "standard";
  defaultBilling?: BillingCycle;
};

function FieldError({ message }: { message?: string }) {
  if (!message) {
    return null;
  }
  return <p className="mt-1 font-sans text-sm text-danger">{message}</p>;
}

export function OnboardingForm({
  locale,
  dictionary,
  defaultPlan = "trial",
  defaultBilling = "monthly",
}: OnboardingFormProps) {
  const [state, formAction, pending] = useActionState(submitOnboarding, initialState);
  const [selectedPlan, setSelectedPlan] = useState<"trial" | "standard">(defaultPlan);
  const defaults = localeDefaults[locale];

  const countryOptions = useMemo(
    () =>
      [...countryCodes]
        .map((code) => ({ code, label: getCountryLabel(locale, code) }))
        .sort((a, b) => a.label.localeCompare(b.label, locale)),
    [locale],
  );

  const timezoneOptions = useMemo(
    () =>
      timezones.map((zone) => ({
        zone,
        label: getTimezoneLabel(locale, zone),
      })),
    [locale],
  );

  if (state.ok) {
    return (
      <div
        className="rounded-md border border-line bg-surface-elevated p-6"
        role="status"
        aria-live="polite"
      >
        <h2 className="font-display text-xl font-semibold text-ink">{dictionary.successTitle}</h2>
        <p className="mt-2 font-sans text-base leading-relaxed text-ink-muted">{state.message}</p>
        <Link
          href={localePath(locale)}
          className="mt-6 inline-flex items-center justify-center rounded-md bg-primary px-4 py-2.5 font-sans text-sm font-semibold text-white transition-colors hover:bg-primary-deep"
        >
          {dictionary.backHome}
        </Link>
      </div>
    );
  }

  return (
    <form action={formAction} className="flex flex-col gap-8" noValidate>
      <input type="hidden" name="locale" value={locale} />

      <fieldset className="flex flex-col gap-5">
        <legend className="font-display text-lg font-semibold text-ink">
          {dictionary.sections.company}
        </legend>

        <div>
          <label htmlFor="companyName" className="block font-sans text-sm font-medium text-ink">
            {dictionary.companyName}
          </label>
          <input
            id="companyName"
            name="companyName"
            type="text"
            autoComplete="organization"
            required
            className={fieldClassName}
          />
          <FieldError message={state.fieldErrors?.companyName} />
        </div>

        <div>
          <label htmlFor="website" className="block font-sans text-sm font-medium text-ink">
            {dictionary.website}{" "}
            <span className="font-normal text-ink-muted">({dictionary.websiteOptional})</span>
          </label>
          <input
            id="website"
            name="website"
            type="url"
            inputMode="url"
            autoComplete="url"
            placeholder="https://"
            className={fieldClassName}
          />
          <FieldError message={state.fieldErrors?.website} />
        </div>

        <div>
          <label htmlFor="companySize" className="block font-sans text-sm font-medium text-ink">
            {dictionary.companySize}
          </label>
          <select id="companySize" name="companySize" required defaultValue="" className={fieldClassName}>
            <option value="" disabled>
              {dictionary.companySizePlaceholder}
            </option>
            {companySizes.map((size) => (
              <option key={size} value={size}>
                {dictionary.companySizes[size as CompanySize]}
              </option>
            ))}
          </select>
          <FieldError message={state.fieldErrors?.companySize} />
        </div>
      </fieldset>

      <fieldset className="flex flex-col gap-5">
        <legend className="font-display text-lg font-semibold text-ink">
          {dictionary.sections.admin}
        </legend>

        <div>
          <label htmlFor="adminName" className="block font-sans text-sm font-medium text-ink">
            {dictionary.adminName}
          </label>
          <input
            id="adminName"
            name="adminName"
            type="text"
            autoComplete="name"
            required
            className={fieldClassName}
          />
          <FieldError message={state.fieldErrors?.adminName} />
        </div>

        <div>
          <label htmlFor="adminEmail" className="block font-sans text-sm font-medium text-ink">
            {dictionary.adminEmail}
          </label>
          <input
            id="adminEmail"
            name="adminEmail"
            type="email"
            autoComplete="email"
            required
            className={fieldClassName}
          />
          <FieldError message={state.fieldErrors?.adminEmail} />
        </div>

        <div>
          <label htmlFor="adminPhone" className="block font-sans text-sm font-medium text-ink">
            {dictionary.adminPhone}{" "}
            <span className="font-normal text-ink-muted">({dictionary.adminPhoneOptional})</span>
          </label>
          <input
            id="adminPhone"
            name="adminPhone"
            type="tel"
            autoComplete="tel"
            placeholder="+1 555 0100"
            className={fieldClassName}
          />
          <FieldError message={state.fieldErrors?.adminPhone} />
        </div>
      </fieldset>

      <fieldset className="flex flex-col gap-5">
        <legend className="font-display text-lg font-semibold text-ink">
          {dictionary.sections.region}
        </legend>

        <div>
          <label htmlFor="country" className="block font-sans text-sm font-medium text-ink">
            {dictionary.country}
          </label>
          <select
            id="country"
            name="country"
            required
            defaultValue={defaults.country}
            className={fieldClassName}
          >
            <option value="" disabled>
              {dictionary.countryPlaceholder}
            </option>
            {countryOptions.map(({ code, label }) => (
              <option key={code} value={code}>
                {label}
              </option>
            ))}
          </select>
          <FieldError message={state.fieldErrors?.country} />
        </div>

        <div>
          <label htmlFor="timezone" className="block font-sans text-sm font-medium text-ink">
            {dictionary.timezone}
          </label>
          <select
            id="timezone"
            name="timezone"
            required
            defaultValue={defaults.timezone}
            className={fieldClassName}
          >
            <option value="" disabled>
              {dictionary.timezonePlaceholder}
            </option>
            {timezoneOptions.map(({ zone, label }) => (
              <option key={zone} value={zone}>
                {label}
              </option>
            ))}
          </select>
          <FieldError message={state.fieldErrors?.timezone} />
        </div>

        <div>
          <label htmlFor="supportLanguage" className="block font-sans text-sm font-medium text-ink">
            {dictionary.supportLanguage}
          </label>
          <select
            id="supportLanguage"
            name="supportLanguage"
            required
            defaultValue={locale}
            className={fieldClassName}
          >
            {locales.map((item) => (
              <option key={item} value={item}>
                {localeLabels[item]}
              </option>
            ))}
          </select>
          <FieldError message={state.fieldErrors?.supportLanguage} />
        </div>
      </fieldset>

      <fieldset>
        <legend className="font-display text-lg font-semibold text-ink">
          {dictionary.sections.plan}
        </legend>
        <div className="mt-4 flex flex-col gap-2 sm:flex-row sm:gap-4">
          <label className="flex cursor-pointer items-center gap-2 rounded-md border border-line bg-surface-elevated px-3 py-2.5 font-sans text-sm text-ink has-[:checked]:border-primary">
            <input
              type="radio"
              name="plan"
              value="trial"
              checked={selectedPlan === "trial"}
              onChange={() => setSelectedPlan("trial")}
              className="accent-primary"
            />
            {dictionary.planTrial}
          </label>
          <label className="flex cursor-pointer items-center gap-2 rounded-md border border-line bg-surface-elevated px-3 py-2.5 font-sans text-sm text-ink has-[:checked]:border-primary">
            <input
              type="radio"
              name="plan"
              value="standard"
              checked={selectedPlan === "standard"}
              onChange={() => setSelectedPlan("standard")}
              className="accent-primary"
            />
            {dictionary.planStandard}
          </label>
        </div>
        <FieldError message={state.fieldErrors?.plan} />

        {selectedPlan === "standard" ? (
          <div className="mt-4">
            <p className="font-sans text-sm font-medium text-ink">{dictionary.billingLegend}</p>
            <div className="mt-2 flex flex-col gap-2">
              <label className="flex cursor-pointer items-center gap-2 rounded-md border border-line bg-surface-elevated px-3 py-2.5 font-sans text-sm text-ink has-[:checked]:border-primary">
                <input
                  type="radio"
                  name="billingCycle"
                  value="monthly"
                  defaultChecked={defaultBilling === "monthly"}
                  className="accent-primary"
                />
                {dictionary.billingMonthly}
              </label>
              <label className="flex cursor-pointer items-center gap-2 rounded-md border border-line bg-surface-elevated px-3 py-2.5 font-sans text-sm text-ink has-[:checked]:border-primary">
                <input
                  type="radio"
                  name="billingCycle"
                  value="annual"
                  defaultChecked={defaultBilling === "annual"}
                  className="accent-primary"
                />
                {dictionary.billingAnnual}
              </label>
            </div>
            <FieldError message={state.fieldErrors?.billingCycle} />
          </div>
        ) : null}
      </fieldset>

      {state.message && !state.ok ? (
        <p className="font-sans text-sm text-danger" role="alert">
          {state.message}
        </p>
      ) : null}

      <button
        type="submit"
        disabled={pending}
        className="inline-flex w-full items-center justify-center rounded-md bg-primary px-5 py-3 font-sans text-sm font-semibold text-white transition-colors hover:bg-primary-deep disabled:cursor-not-allowed disabled:opacity-70"
      >
        {pending ? dictionary.submitting : dictionary.submit}
      </button>
    </form>
  );
}
