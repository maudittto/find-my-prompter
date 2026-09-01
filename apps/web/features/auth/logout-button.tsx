"use client";

import { useRouter } from "next/navigation";
import { useState } from "react";

import { Button } from "@/components/ui/button";

export function LogoutButton() {
  const router = useRouter();
  const [pending, setPending] = useState(false);

  async function handleClick() {
    setPending(true);

    await fetch("/api/auth/logout", { method: "POST" });

    router.push("/login");
    router.refresh();
  }

  return (
    <Button variant="outline" onClick={handleClick} disabled={pending}>
      {pending ? "Saindo..." : "Sair"}
    </Button>
  );
}
