"use client";

import { localeLabels, localePath, locales, type Locale } from "@/lib/i18n";
import { usePathname, useRouter } from "next/navigation";
import {
  useEffect,
  useId,
  useLayoutEffect,
  useRef,
  useState,
  useSyncExternalStore,
} from "react";
import { createPortal } from "react-dom";

type LocaleSwitcherProps = {
  label: string;
  currentLocale: Locale;
};

const localeFlags: Record<Locale, string> = {
  "en-us": "🇺🇸",
  "pt-br": "🇧🇷",
  "es-es": "🇪🇸",
};

function useIsClient() {
  return useSyncExternalStore(
    () => () => {},
    () => true,
    () => false,
  );
}

function pathForLocale(pathname: string, nextLocale: Locale): string {
  const segments = pathname.split("/");
  if (segments.length > 1) {
    segments[1] = nextLocale;
    return segments.join("/") || `/${nextLocale}`;
  }
  return localePath(nextLocale);
}

export function LocaleSwitcher({ label, currentLocale }: LocaleSwitcherProps) {
  const pathname = usePathname();
  const router = useRouter();
  const isClient = useIsClient();
  const [open, setOpen] = useState(false);
  const buttonRef = useRef<HTMLButtonElement>(null);
  const menuRef = useRef<HTMLUListElement>(null);
  const listId = useId();
  const [menuStyle, setMenuStyle] = useState<React.CSSProperties>({
    position: "fixed",
    top: 0,
    right: 8,
    zIndex: 100,
  });

  useLayoutEffect(() => {
    if (!open || !buttonRef.current) {
      return;
    }

    function placeMenu() {
      const rect = buttonRef.current?.getBoundingClientRect();
      if (!rect) {
        return;
      }
      setMenuStyle({
        position: "fixed",
        top: rect.bottom + 6,
        right: Math.max(8, window.innerWidth - rect.right),
        zIndex: 100,
      });
    }

    placeMenu();
    window.addEventListener("resize", placeMenu);
    window.addEventListener("scroll", placeMenu, true);
    return () => {
      window.removeEventListener("resize", placeMenu);
      window.removeEventListener("scroll", placeMenu, true);
    };
  }, [open]);

  useEffect(() => {
    if (!open) {
      return;
    }

    function onPointerDown(event: MouseEvent) {
      const target = event.target as Node;
      if (buttonRef.current?.contains(target) || menuRef.current?.contains(target)) {
        return;
      }
      setOpen(false);
    }

    function onKeyDown(event: KeyboardEvent) {
      if (event.key === "Escape") {
        setOpen(false);
      }
    }

    document.addEventListener("mousedown", onPointerDown);
    document.addEventListener("keydown", onKeyDown);
    return () => {
      document.removeEventListener("mousedown", onPointerDown);
      document.removeEventListener("keydown", onKeyDown);
    };
  }, [open]);

  function onSelect(nextLocale: Locale) {
    setOpen(false);
    if (nextLocale === currentLocale) {
      return;
    }
    router.push(pathForLocale(pathname, nextLocale));
  }

  return (
    <div className="relative">
      <button
        ref={buttonRef}
        type="button"
        aria-label={label}
        aria-haspopup="listbox"
        aria-expanded={open}
        aria-controls={listId}
        title={localeLabels[currentLocale]}
        onClick={() => setOpen((value) => !value)}
        className="inline-flex size-9 items-center justify-center rounded-md border border-line bg-surface-elevated text-base leading-none transition-colors hover:border-primary"
      >
        <span aria-hidden>{localeFlags[currentLocale]}</span>
      </button>

      {isClient && open
        ? createPortal(
            <ul
              ref={menuRef}
              id={listId}
              role="listbox"
              aria-label={label}
              style={menuStyle}
              className="min-w-40 overflow-hidden rounded-md border border-line bg-surface-elevated py-1 shadow-lg"
            >
              {locales.map((locale) => {
                const selected = locale === currentLocale;
                return (
                  <li key={locale} role="option" aria-selected={selected}>
                    <button
                      type="button"
                      onClick={() => onSelect(locale)}
                      className={`flex w-full items-center gap-2.5 px-3 py-2 text-left font-sans text-sm transition-colors ${
                        selected
                          ? "bg-primary/10 font-semibold text-primary"
                          : "text-ink hover:bg-surface"
                      }`}
                    >
                      <span className="text-base leading-none" aria-hidden>
                        {localeFlags[locale]}
                      </span>
                      <span>{localeLabels[locale]}</span>
                    </button>
                  </li>
                );
              })}
            </ul>,
            document.body,
          )
        : null}
    </div>
  );
}
