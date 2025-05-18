import { Routes, Route } from "react-router-dom";

// Pages
import Homepage from "../pages/homepage/homepage";
import NotFound from "../pages/notfound/notfound"; // Assuming a NotFound component exists

const AppRoutes = () => {
    return (
        <Routes>
            <Route path="/" element={<Homepage />} />
            <Route path="*" element={<NotFound />} />
        </Routes>
    );
}

export default AppRoutes;
