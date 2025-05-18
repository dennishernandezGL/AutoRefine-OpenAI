import { StrictMode } from 'react';
import { createRoot } from 'react-dom/client';
import App from './App.tsx';
import mixpanel from 'mixpanel-browser';

mixpanel.init('YOUR_TOKEN_HERE'); // Initialize MixPanel with your project's token

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <App />
  </StrictMode>,
);