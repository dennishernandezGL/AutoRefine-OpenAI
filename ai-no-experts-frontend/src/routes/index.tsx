import { Routes, Route } from "react-router-dom";

// Pages
import Homepage from "../pages/homepage/homepage";
import NotFound from "../pages/notfound/notfound";

const AppRoutes = () => {
    return (
        <Routes>
            <Route path="/" element={<Homepage />} />
            <Route path="*" element={<NotFound />} />
        </Routes>
    );
}

export default AppRoutes;
