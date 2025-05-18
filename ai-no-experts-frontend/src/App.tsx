
import { BrowserRouter } from 'react-router-dom';
import CssBaseline from '@mui/material/CssBaseline';

import { MixpanelProvider } from './mixpanel'; // Assuming you have a Mixpanel provider or context
import AppRoutes from './routes';

import './scss/main.scss';

function App() {
  return (
    <MixpanelProvider> 
      {/* Wrap application with Mixpanel Provider for potential mixpanel tracking session */}
      <CssBaseline />
      <BrowserRouter>
        <AppRoutes />
      </BrowserRouter>
    </MixpanelProvider>
  );
}

export default App;
