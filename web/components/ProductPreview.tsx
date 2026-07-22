export function ProductPreview() {
  return (
    <div
      className="relative h-full min-h-0 w-full overflow-hidden border-t border-line/80 bg-[#0f172a] lg:border-t-0 lg:border-l"
      aria-hidden="true"
    >
      <div className="absolute inset-0 bg-[radial-gradient(ellipse_at_top_right,rgba(37,99,235,0.35),transparent_55%)]" />
      <div className="absolute inset-0 flex">
        <aside className="flex w-14 shrink-0 flex-col gap-2 border-r border-white/10 bg-[#0f172a]/80 p-2 sm:w-16">
          <div className="mb-2 h-6 rounded-sm bg-primary/40" />
          <div className="h-7 rounded-sm bg-white/10" />
          <div className="h-7 rounded-sm bg-white/10" />
          <div className="h-7 rounded-sm bg-primary/25" />
          <div className="h-7 rounded-sm bg-white/10" />
        </aside>
        <div className="flex flex-1 flex-col gap-3 p-3 sm:p-4">
          <div className="flex items-center gap-2">
            <div className="h-8 w-8 rounded-sm bg-white/15" />
            <div className="h-8 flex-1 rounded-sm bg-white/10" />
          </div>
          <div className="grid grid-cols-2 gap-2 sm:gap-3">
            <div className="rounded-md border border-white/10 bg-white/5 p-3">
              <div className="mb-2 h-4 w-3/5 rounded-sm bg-primary/40" />
              <div className="h-3 w-2/5 rounded-sm bg-white/20" />
            </div>
            <div className="rounded-md border border-white/10 bg-white/5 p-3">
              <div className="mb-2 h-4 w-1/2 rounded-sm bg-primary-soft/50" />
              <div className="h-3 w-1/3 rounded-sm bg-white/20" />
            </div>
            <div className="col-span-2 rounded-md border border-white/10 bg-white/5 p-3">
              <div className="mb-2 h-2.5 w-full rounded-sm bg-white/15" />
              <div className="mb-2 h-2.5 w-[90%] rounded-sm bg-white/15" />
              <div className="h-2.5 w-[70%] rounded-sm bg-white/15" />
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
