"use client";

import { useActionState } from "react";
import Link from "next/link";
import {
  submitForgotPassword,
  type ForgotPasswordState,
} from "@/app/[locale]/forgot-password/actions";
import type { Dictionary } from "@/lib/dictionary";
import { localePath, type Locale } from "@/lib/i18n";

const initialState: ForgotPasswordState = { ok: false };

const fieldClassName =
  "mt-1.5 w-full rounded-md border border-line bg-surface-elevated px-3 py-2.5 font-sans text-sm text-ink outline-none transition-colors focus:border-primary";

type ForgotPasswordFormProps = {
  locale: Locale;
  dictionary: Dictionary["forgotPassword"];
};

export function ForgotPasswordForm({ locale, dictionary }: ForgotPasswordFormProps) {
  const [state, formAction, pending] = useActionState(submitForgotPassword, initialState);

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
          href={localePath(locale, "/login")}
          className="mt-6 inline-flex items-center justify-center rounded-md bg-primary px-4 py-2.5 font-sans text-sm font-semibold text-white transition-colors hover:bg-primary-deep"
        >
          {dictionary.backToLogin}
        </Link>
      </div>
    );
  }

  return (
    <form action={formAction} className="flex flex-col gap-5" noValidate>
      <input type="hidden" name="locale" value={locale} />

      <div>
        <label htmlFor="email" className="block font-sans text-sm font-medium text-ink">
          {dictionary.email}
        </label>
        <input
          id="email"
          name="email"
          type="email"
          autoComplete="email"
          required
          className={fieldClassName}
        />
        {state.fieldErrors?.email ? (
          <p className="mt-1 font-sans text-sm text-danger">{state.fieldErrors.email}</p>
        ) : null}
      </div>

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

      <p className="font-sans text-sm text-ink-muted">
        <Link
          href={localePath(locale, "/login")}
          className="font-semibold text-primary transition-colors hover:text-primary-deep"
        >
          {dictionary.backToLogin}
        </Link>
      </p>
    </form>
  );
}
