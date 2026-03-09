import { Navigation } from '@/components/navigation';

export default function HomePage() {
  return (
      <div>
        <Navigation />
        <main className="p-8">
          <h1 className="text-3xl font-bold mb-8">Главная</h1>
        </main>
      </div>
  );
}