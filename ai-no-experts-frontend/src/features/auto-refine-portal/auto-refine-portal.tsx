import { Box, Container } from "@mui/material";
import { useState } from "react";

import { submitPayment } from "../../api/payment/payment.service";
import Loading from "../../components/Loading/Loading";
import PaymentForm from "../../components/PaymentForm/PaymentForm";
import SnackbarComponent from "../../components/Snackbar/Snackbar";
import type { Payment } from "../../models/payment.model";

const AutoRefinePortal = () => {
    const [isLoading, setIsLoading] = useState<boolean>(false);
    const [errorMessage, setErrorMessage] = useState<string>('');
    const [successMessage, setSuccessMessage] = useState<string>('');

    const onPaymentFormSubmit = async (values: Payment) => {
        try {
            setIsLoading(true);
            setErrorMessage('');
            
            const response = await submitPayment(values);
            switch(response.statusCode) {
                case 200:
                    setSuccessMessage(response.data);
                    break;
                default:
                    setErrorMessage(formatErrorMessages(response.data?.errors));
                    break;
            }

        } catch (error: any) {
            console.error('Submission error:', error);
            setErrorMessage(`Error: ${error.message}`);
        } finally {
            setIsLoading(false);
        }
    }

    const formatErrorMessages = (errors: any) => {
        if (!errors) {
            return 'An unknown error occurred.';
        }

        return Object.entries(errors)
           .map(([field, messages]: any) => `${field}: ${messages.join(', ')}`)
           .join(' | ');
    }

    return (
        <Container maxWidth='lg'>
            {/* Payment Form */}
            <Box>
                <PaymentForm onSubmit={(values: any) => onPaymentFormSubmit(values)} />
            </Box>

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
                message={errorMessage}
                autoHideDuration={5000}
                onClose={() => setErrorMessage('')} 
            />
        </Container>
    );
}

export default AutoRefinePortal;
