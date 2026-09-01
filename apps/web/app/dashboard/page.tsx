import { cookies } from "next/headers";
import { redirect } from "next/navigation";

import { LogoutButton } from "@/features/auth/logout-button";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";

export const metadata = { title: "Dashboard" };

export default async function DashboardPage() {
  const response = await fetch(`${process.env.API_URL}/api/me`, {
    headers: { cookie: (await cookies()).toString() },
    cache: "no-store",
  });

  if (!response.ok) {
    redirect("/login");
  }

  const user: { authenticated: boolean; name: string | null } = await response.json();

  return (
    <main className="flex flex-1 items-center justify-center p-6">
      <Card className="w-full max-w-sm">
        <CardHeader>
          <CardTitle>Sessão ativa</CardTitle>
          <CardDescription>Dados vindos de GET /api/me.</CardDescription>
        </CardHeader>
        <CardContent className="grid gap-4">
          <dl className="grid gap-1 text-sm">
            <div className="flex justify-between">
              <dt className="text-muted-foreground">Usuário</dt>
              <dd>{user.name ?? "-"}</dd>
            </div>
            <div className="flex justify-between">
              <dt className="text-muted-foreground">Autenticado</dt>
              <dd>{String(user.authenticated)}</dd>
            </div>
          </dl>
          <LogoutButton />
        </CardContent>
      </Card>
    </main>
  );
}
