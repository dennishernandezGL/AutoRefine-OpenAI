import { Box, Container } from "@mui/material";
import { useState } from "react";

import { submitPayment } from "../../api/payment/payment.service";
import Loading from "../../components/Loading/Loading";
import PaymentForm from "../../components/PaymentForm/PaymentForm";
// import Recommendations from "../../components/Recommendations/Recommendations";
import SnackbarComponent from "../../components/Snackbar/Snackbar";
import type { Payment } from "../../models/payment.model";
// import type { Recommendation } from "../../models/log.model";

const AutoRefinePortal = () => {
    const [isLoading, setIsLoading] = useState<boolean>(false);
    const [errorMessage, setErrorMessage] = useState<string>('');
    const [successMessage, setSuccessMessage] = useState<string>('');

    // const [recommendations, setRecommendations] = useState<Recommendation[]>([]);

    const onPaymentFormSubmit = async (values: Payment) => {             
        try {
            setIsLoading(true);
            setErrorMessage('');
            
            const response = await submitPayment(values);
            
            if (response.statusCode === 200) {
                if (response.data?.message) {
                    setSuccessMessage(response.data.message);
                } else {
                    setSuccessMessage("Payment processed successfully.");
                }
            } else {
                const errors = response.data?.errors;
                if (errors) {
                    const errorMessages = Object.entries(errors)
                        .map(([field, messages]: any) => `${field}: ${messages.join(', ')}`)
                        .join(' | ');

                    setErrorMessage(errorMessages);
                } else {
                    setErrorMessage("An unknown error occurred.");
                }
            }

            setIsLoading(false);
        } catch (error: any) {
            setIsLoading(false);
            console.error('Submission error:', error);
            setErrorMessage(`Error: ${error.message}`);
        }
    }

    return (
        <Container maxWidth='lg'>
            {/* Payment Form */}
            <Box>
                <PaymentForm onSubmit={(values: any) => onPaymentFormSubmit(values)} />
            </Box>

            {/* Recommendations */}
            {/* {recommendations.length > 0 && (
                <Box sx={{ marginTop: '50px' }}>
                    <Recommendations recommendations={recommendations} />
                </Box>
            )} */}

            <Loading isOpen={isLoading} />

            {/* Success Message */}
            <SnackbarComponent 
                open={!!successMessage}
                message={successMessage}
                severity="success"
                autoHideDuration={5000}
                onClose={() => setSuccessMessage('')} 
            />

            {/* Error Message */}
            <SnackbarComponent 
                open={!!errorMessage}
                message={errorMessage || ''}
                autoHideDuration={5000}
                onClose={() => setErrorMessage('')} 
            />
        </Container>
    );
}

export default AutoRefinePortal;
