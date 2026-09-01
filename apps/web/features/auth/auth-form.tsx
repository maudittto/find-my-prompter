"use client";

import { useRouter } from "next/navigation";
import { useState } from "react";

import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";

type Mode = "login" | "register";

async function readError(response: Response) {
  try {
    const problem = await response.json();
    const details = problem?.errors
      ? Object.values(problem.errors as Record<string, string[]>).flat()
      : [];

    if (details.length > 0) return details.join(" ");
    if (problem?.detail) return String(problem.detail);
    if (problem?.title) return String(problem.title);
  } catch {
    // resposta sem corpo JSON
  }

  return response.status === 401
    ? "E-mail ou senha inválidos."
    : `Falha na requisição (${response.status}).`;
}

export function AuthForm({ mode }: { mode: Mode }) {
  const router = useRouter();
  const [error, setError] = useState<string | null>(null);
  const [pending, setPending] = useState(false);

  async function handleSubmit(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setError(null);
    setPending(true);

    const form = new FormData(event.currentTarget);
    const url =
      mode === "login" ? "/api/auth/login?useCookies=true" : "/api/auth/register";

    try {
      const response = await fetch(url, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
          email: form.get("email"),
          password: form.get("password"),
        }),
      });

      if (!response.ok) {
        setError(await readError(response));
        return;
      }

      router.push(mode === "login" ? "/dashboard" : "/login");
      router.refresh();
    } catch {
      setError("Não foi possível falar com a API. Ela está rodando?");
    } finally {
      setPending(false);
    }
  }

  return (
    <form onSubmit={handleSubmit} className="grid gap-4">
      <div className="grid gap-2">
        <Label htmlFor="email">E-mail</Label>
        <Input id="email" name="email" type="email" required autoComplete="email" />
      </div>

      <div className="grid gap-2">
        <Label htmlFor="password">Senha</Label>
        <Input
          id="password"
          name="password"
          type="password"
          required
          minLength={6}
          autoComplete={mode === "login" ? "current-password" : "new-password"}
        />
      </div>

      {error ? (
        <p role="alert" className="text-sm text-destructive">
          {error}
        </p>
      ) : null}

      <Button type="submit" disabled={pending}>
        {pending ? "Enviando..." : mode === "login" ? "Entrar" : "Cadastrar"}
      </Button>
    </form>
  );
}
