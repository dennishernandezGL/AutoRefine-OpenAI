// Import necessary modules
import { BrowserRouter } from 'react-router-dom';
import CssBaseline from '@mui/material/CssBaseline';

// Import application routes
import AppRoutes from './routes';

// Import global styles
import './scss/main.scss'

// Main App component
function App() {
  return (
    <>
      {/* Provides a consistent baseline style for the application */}
      <CssBaseline />
      <BrowserRouter>
        {/* Render application routes */}
        <AppRoutes />
      </BrowserRouter>
    </>
  )
}

// Export component as default
export default App;
