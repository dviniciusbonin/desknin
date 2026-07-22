type LogoProps = {
  className?: string;
  size?: "sm" | "lg";
  tone?: "default" | "onDark";
};

export function Logo({ className = "", size = "sm", tone = "default" }: LogoProps) {
  const markClass =
    size === "lg"
      ? "font-display text-3xl font-semibold tracking-tight sm:text-5xl md:text-6xl"
      : "font-display text-xl font-semibold tracking-tight";

  const deskClass = tone === "onDark" ? "text-slate-300" : "text-ink-muted";
  const ninClass = tone === "onDark" ? "text-primary-soft" : "text-primary";

  return (
    <span className={`inline-flex items-baseline leading-none ${markClass} ${className}`}>
      <span className={deskClass}>Desk</span>
      <span className={ninClass}>
        N<span className="wordmark-i">i</span>n
      </span>
    </span>
  );
}
