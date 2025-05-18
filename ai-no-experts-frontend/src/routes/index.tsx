import { Routes, Route } from "react-router-dom";

// Pages
import Homepage from "../pages/homepage/homepage";
import NotFoundPage from "../pages/notfound/notfound"; // Assuming you have a NotFoundPage defined

const AppRoutes = () => {
    return (
        <Routes>
            <Route path="/" element={<Homepage />} />
            <Route path="*" element={<NotFoundPage />} />
        </Routes>
    );
}

export default AppRoutes;
