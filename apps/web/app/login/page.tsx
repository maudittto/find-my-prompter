import Link from "next/link";

import { AuthForm } from "@/features/auth/auth-form";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";

export const metadata = { title: "Entrar" };

export default function LoginPage() {
  return (
    <main className="flex flex-1 items-center justify-center p-6">
      <Card className="w-full max-w-sm">
        <CardHeader>
          <CardTitle>Entrar</CardTitle>
          <CardDescription>Use sua conta para acessar o dashboard.</CardDescription>
        </CardHeader>
        <CardContent className="grid gap-4">
          <AuthForm mode="login" />
          <p className="text-sm text-muted-foreground">
            Não tem conta?{" "}
            <Link href="/register" className="underline">
              Cadastre-se
            </Link>
          </p>
        </CardContent>
      </Card>
    </main>
  );
}
