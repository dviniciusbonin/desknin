"use client";

import { ChevronUpIcon } from "@/components/icons";
import { useSyncExternalStore } from "react";

type BackToTopProps = {
  label: string;
};

function subscribe(onStoreChange: () => void) {
  window.addEventListener("scroll", onStoreChange, { passive: true });
  return () => window.removeEventListener("scroll", onStoreChange);
}

function getSnapshot() {
  return window.scrollY > 400;
}

function getServerSnapshot() {
  return false;
}

export function BackToTop({ label }: BackToTopProps) {
  const visible = useSyncExternalStore(subscribe, getSnapshot, getServerSnapshot);

  return (
    <button
      type="button"
      aria-label={label}
      title={label}
      onClick={() => window.scrollTo({ top: 0, behavior: "smooth" })}
      className={`fixed right-5 z-50 inline-flex size-11 items-center justify-center rounded-md border border-line bg-surface-elevated text-ink shadow-sm transition-all duration-200 hover:border-primary hover:text-primary sm:right-8 ${
        visible
          ? "translate-y-0 opacity-100"
          : "pointer-events-none translate-y-2 opacity-0"
      }`}
      style={{ bottom: "max(1.25rem, env(safe-area-inset-bottom, 0px))" }}
    >
      <ChevronUpIcon className="size-5" />
    </button>
  );
}
