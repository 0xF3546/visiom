import { QueryClientProvider } from '@tanstack/react-query';
import AppRouter from './AppRouter';
import { queryClient } from './utils/queryClien';

function App() {
  return (
    <QueryClientProvider client={queryClient}>
    <AppRouter />
    </QueryClientProvider>
  );
}

export default App;
