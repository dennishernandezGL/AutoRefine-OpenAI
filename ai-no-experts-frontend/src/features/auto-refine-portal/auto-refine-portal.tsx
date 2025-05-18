import React, { useState, useCallback } from "react";
import { Box, Container } from "@mui/material";

import { submitPayment } from "../../api/payment/payment.service";
import Loading from "../../components/Loading/Loading";
import PaymentForm from "../../components/PaymentForm/PaymentForm";
import SnackbarComponent from "../../components/Snackbar/Snackbar";
import type { Payment } from "../../models/payment.model";

const AutoRefinePortal = () => {
    const [isLoading, setIsLoading] = useState<boolean>(false);
    const [errorMessage, setErrorMessage] = useState<string>('');
    const [successMessage, setSuccessMessage] = useState<string>('');

    const onPaymentFormSubmit = useCallback(async (values: Payment) => {             
        try {
            setIsLoading(true);
            setErrorMessage('');
            
            const response = await submitPayment(values);
            if (response.statusCode === 200) {
                setSuccessMessage(response.data);
            } else {
                const errors = response.data?.errors;
                if (errors) {
                    const errorMessages = Object.entries(errors)
                        .map(([field, messages]: [string, string[]]) => `${field}: ${messages.join(', ')}`)
                        .join(' | ');

                    setErrorMessage(errorMessages);
                } else {
                    setErrorMessage("An unknown error occurred.");
                }
            }

        } catch (error: any) {
            console.error('Submission error:', error);
            setErrorMessage(`Error: ${error.message || 'Server error occurred'}`);
        } finally {
            setIsLoading(false);
        }
    }, []);

    return (
        <Container maxWidth='lg'>
            <Box>
                <PaymentForm onSubmit={(values: Payment) => onPaymentFormSubmit(values)} />
            </Box>

            <Loading isOpen={isLoading} />

            <SnackbarComponent 
                open={!!successMessage}
                message={successMessage}
                severity="success"
                autoHideDuration={5000}
                onClose={() => setSuccessMessage('')} 
            />

            <SnackbarComponent 
                open={!!errorMessage}
                message={errorMessage}
                severity="error"
                autoHideDuration={5000}
                onClose={() => setErrorMessage('')} 
            />
        </Container>
    );
}

export default AutoRefinePortal;
