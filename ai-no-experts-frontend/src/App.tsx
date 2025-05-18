import { BrowserRouter } from 'react-router-dom';
import CssBaseline from '@mui/material/CssBaseline';
import { ThemeProvider, createTheme } from '@mui/material/styles';

import AppRoutes from './routes';

import './scss/main.scss'

const theme = createTheme({
  // Define your custom theme properties here
});

function App() {
  return (
    <ThemeProvider theme={theme}>
      <CssBaseline />
      <BrowserRouter>
        <AppRoutes />
      </BrowserRouter>
    </ThemeProvider>
  )
}

export default App

import CssBaseline from '@mui/material/CssBaseline';

import AppRoutes from './routes';

import './scss/main.scss'

function App() {
  return (
    <>
      <CssBaseline />
      <BrowserRouter>
        <AppRoutes />
      </BrowserRouter>
    </>
  )
}

export default App
