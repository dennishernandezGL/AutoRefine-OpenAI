import { Routes, Route } from "react-router-dom";

// Pages
import Homepage from "../pages/homepage/homepage";

// Create a NotFound component to handle undefined routes
const NotFound = () => <div>404 - Page Not Found</div>;

const AppRoutes = () => {
    return (
        <Routes>
            <Route path="/" element={<Homepage />} />
            <Route path="*" element={<NotFound />} />
        </Routes>
    );
}

export default AppRoutes;