"use client";

import { useActionState, useEffect, useState } from "react";
import Link from "next/link";
import { useRouter } from "next/navigation";
import { submitLogin, type LoginState } from "@/app/[locale]/login/actions";
import { EyeIcon, EyeOffIcon } from "@/components/icons";
import type { Dictionary } from "@/lib/dictionary";
import { localePath, type Locale } from "@/lib/i18n";

const initialState: LoginState = { ok: false };

const fieldClassName =
  "mt-1.5 w-full rounded-md border border-line bg-surface-elevated px-3 py-2.5 font-sans text-sm text-ink outline-none transition-colors focus:border-primary";

type LoginFormProps = {
  locale: Locale;
  dictionary: Dictionary["login"];
};

function FieldError({ message }: { message?: string }) {
  if (!message) {
    return null;
  }
  return <p className="mt-1 font-sans text-sm text-danger">{message}</p>;
}

export function LoginForm({ locale, dictionary }: LoginFormProps) {
  const router = useRouter();
  const [state, formAction, pending] = useActionState(submitLogin, initialState);
  const [showPassword, setShowPassword] = useState(false);

  useEffect(() => {
    if (state.ok && state.redirectTo) {
      router.push(state.redirectTo);
    }
  }, [state.ok, state.redirectTo, router]);

  if (state.ok && !state.redirectTo) {
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
        <FieldError message={state.fieldErrors?.email} />
      </div>

      <div>
        <div className="flex items-center justify-between gap-3">
          <label htmlFor="password" className="block font-sans text-sm font-medium text-ink">
            {dictionary.password}
          </label>
          <Link
            href={localePath(locale, "/forgot-password")}
            className="font-sans text-xs font-medium text-primary transition-colors hover:text-primary-deep"
          >
            {dictionary.forgotPassword}
          </Link>
        </div>
        <div className="relative mt-1.5">
          <input
            id="password"
            name="password"
            type={showPassword ? "text" : "password"}
            autoComplete="current-password"
            required
            className={`${fieldClassName} mt-0 pr-11`}
          />
          <button
            type="button"
            onClick={() => setShowPassword((value) => !value)}
            aria-label={showPassword ? dictionary.hidePassword : dictionary.showPassword}
            className="absolute top-1/2 right-2.5 -translate-y-1/2 rounded-sm p-1 text-ink-muted transition-colors hover:text-ink"
          >
            {showPassword ? <EyeOffIcon className="size-4" /> : <EyeIcon className="size-4" />}
          </button>
        </div>
        <FieldError message={state.fieldErrors?.password} />
      </div>

      <label className="flex cursor-pointer items-center gap-2 font-sans text-sm text-ink">
        <input type="checkbox" name="rememberMe" className="accent-primary" />
        {dictionary.rememberMe}
      </label>

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
        {dictionary.noAccount}{" "}
        <Link
          href={localePath(locale, "/onboarding")}
          className="font-semibold text-primary transition-colors hover:text-primary-deep"
        >
          {dictionary.createWorkspace}
        </Link>
      </p>
    </form>
  );
}
