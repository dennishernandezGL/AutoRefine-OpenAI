import { BrowserRouter } from 'react-router-dom';
import CssBaseline from '@mui/material/CssBaseline';
import { StrictMode } from 'react';

import AppRoutes from './routes';

import './scss/main.scss'

function App() {
  return (
    <StrictMode>
      <CssBaseline />
      <BrowserRouter>
        <AppRoutes />
      </BrowserRouter>
    </StrictMode>
  )
}

export default App
