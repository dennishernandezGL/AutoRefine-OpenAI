
import { Routes, Route, Navigate } from "react-router-dom";

// Pages
import Homepage from "../pages/homepage/homepage";

const AppRoutes = () => {
    return (
        <Routes>
            <Route path="/" element={<Homepage />} />
            <Route path="*" element={<Navigate to="/" replace />} />
        </Routes>
    );
}

export default AppRoutes;
