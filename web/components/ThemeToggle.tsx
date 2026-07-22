"use client";

import { MoonIcon, SunIcon } from "@/components/icons";
import { useTheme } from "next-themes";
import { useSyncExternalStore } from "react";

type ThemeToggleProps = {
  label: string;
  lightLabel: string;
  darkLabel: string;
};

function useIsClient() {
  return useSyncExternalStore(
    () => () => {},
    () => true,
    () => false,
  );
}

export function ThemeToggle({ label, lightLabel, darkLabel }: ThemeToggleProps) {
  const { theme, setTheme } = useTheme();
  const isClient = useIsClient();

  if (!isClient) {
    return (
      <span className="inline-flex size-9 rounded-md border border-line bg-surface-elevated" aria-hidden />
    );
  }

  const isDark = theme === "dark";
  const nextLabel = isDark ? lightLabel : darkLabel;

  return (
    <button
      type="button"
      aria-label={`${label}: ${nextLabel}`}
      title={nextLabel}
      onClick={() => setTheme(isDark ? "light" : "dark")}
      className="inline-flex size-9 items-center justify-center rounded-md border border-line bg-surface-elevated text-ink transition-colors hover:border-primary hover:text-primary"
    >
      {isDark ? <MoonIcon className="size-4" /> : <SunIcon className="size-4" />}
    </button>
  );
}
