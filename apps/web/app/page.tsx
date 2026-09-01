import Link from "next/link";

import { buttonVariants } from "@/components/ui/button";

export default function Home() {
  return (
    <main className="flex flex-1 flex-col items-center justify-center gap-6 p-6">
      <h1 className="text-2xl font-semibold">Find My Prompter</h1>
      <div className="flex gap-3">
        <Link href="/login" className={buttonVariants()}>
          Entrar
        </Link>
        <Link href="/register" className={buttonVariants({ variant: "outline" })}>
          Cadastrar
        </Link>
        <Link href="/dashboard" className={buttonVariants({ variant: "ghost" })}>
          Dashboard
        </Link>
      </div>
    </main>
  );
}
