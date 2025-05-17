import { Routes, Route } from "react-router-dom";

// Pages
import Homepage from "../pages/homepage/homepage";

const AppRoutes = () => {
    return (
        <Routes>
            <Route path="/" element={<Homepage />} />
            <Route path="*" element={<Homepage />} />
        </Routes>
    );
}

export default AppRoutes;