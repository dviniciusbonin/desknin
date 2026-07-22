"use client";

import Link from "next/link";
import { useEffect, useId, useRef, useState, useSyncExternalStore } from "react";
import { createPortal } from "react-dom";
import { CloseIcon, MenuIcon } from "@/components/icons";
import type { Dictionary } from "@/lib/dictionary";
import { localePath, type Locale } from "@/lib/i18n";

type MobileNavProps = {
  locale: Locale;
  dictionary: Dictionary;
};

function useIsClient() {
  return useSyncExternalStore(
    () => () => {},
    () => true,
    () => false,
  );
}

export function MobileNav({ locale, dictionary }: MobileNavProps) {
  const isClient = useIsClient();
  const [open, setOpen] = useState(false);
  const panelId = useId();
  const home = localePath(locale);
  const closeButtonRef = useRef<HTMLButtonElement>(null);
  const triggerRef = useRef<HTMLButtonElement>(null);

  useEffect(() => {
    if (!open) {
      return;
    }

    const previous = document.body.style.overflow;
    const trigger = triggerRef.current;
    document.body.style.overflow = "hidden";
    closeButtonRef.current?.focus();

    function onKeyDown(event: KeyboardEvent) {
      if (event.key === "Escape") {
        setOpen(false);
      }
    }

    document.addEventListener("keydown", onKeyDown);
    return () => {
      document.body.style.overflow = previous;
      document.removeEventListener("keydown", onKeyDown);
      trigger?.focus();
    };
  }, [open]);

  const links = [
    { href: `${home}#features`, label: dictionary.nav.features },
    { href: `${home}#plans`, label: dictionary.nav.plans },
    { href: `${home}#faq`, label: dictionary.nav.faq },
    { href: `${home}#contact`, label: dictionary.nav.contact },
    { href: localePath(locale, "/login"), label: dictionary.nav.signIn },
  ];

  return (
    <>
      <button
        ref={triggerRef}
        type="button"
        className="inline-flex size-9 items-center justify-center rounded-md border border-line bg-surface-elevated text-ink transition-colors hover:border-primary hover:text-primary xl:hidden"
        aria-label={open ? dictionary.nav.closeMenu : dictionary.nav.openMenu}
        aria-expanded={open}
        aria-controls={panelId}
        onClick={() => setOpen((value) => !value)}
      >
        {open ? <CloseIcon className="size-4" /> : <MenuIcon className="size-4" />}
      </button>

      {isClient && open
        ? createPortal(
            <div
              id={panelId}
              className="fixed inset-0 z-[60] xl:hidden"
              role="dialog"
              aria-modal="true"
              aria-label={dictionary.nav.menu}
            >
              <button
                type="button"
                className="absolute inset-0 bg-ink/40"
                aria-label={dictionary.nav.closeMenu}
                onClick={() => setOpen(false)}
              />
              <div className="absolute inset-x-0 top-0 border-b border-line bg-surface-elevated px-5 pt-[max(1.25rem,env(safe-area-inset-top))] pb-6 shadow-lg sm:px-8">
                <div className="mx-auto flex w-full max-w-6xl items-center justify-between gap-4">
                  <p className="font-display text-lg font-semibold text-ink">{dictionary.nav.menu}</p>
                  <button
                    ref={closeButtonRef}
                    type="button"
                    className="inline-flex size-9 items-center justify-center rounded-md border border-line text-ink transition-colors hover:border-primary hover:text-primary"
                    aria-label={dictionary.nav.closeMenu}
                    onClick={() => setOpen(false)}
                  >
                    <CloseIcon className="size-4" />
                  </button>
                </div>
                <nav aria-label={dictionary.nav.menu} className="mx-auto mt-6 flex w-full max-w-6xl flex-col gap-1">
                  {links.map((link) => (
                    <a
                      key={link.href}
                      href={link.href}
                      onClick={() => setOpen(false)}
                      className="rounded-md px-3 py-3 font-sans text-base font-medium text-ink transition-colors hover:bg-surface hover:text-primary"
                    >
                      {link.label}
                    </a>
                  ))}
                  <Link
                    href={localePath(locale, "/onboarding")}
                    onClick={() => setOpen(false)}
                    className="mt-3 inline-flex items-center justify-center rounded-md bg-primary px-4 py-3 font-sans text-sm font-semibold text-white transition-colors hover:bg-primary-deep"
                  >
                    {dictionary.nav.getStarted}
                  </Link>
                </nav>
              </div>
            </div>,
            document.body,
          )
        : null}
    </>
  );
}
