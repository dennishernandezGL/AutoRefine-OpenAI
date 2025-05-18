import { FormControl, InputAdornment, TextField } from '@mui/material';
import { Field, ErrorMessage } from 'formik';
import { type FunctionComponent } from 'react';

const FormField: FunctionComponent<FormFieldProps> = ({
    endAdornment,    
    fullWidth = true,
    label = '',
    name = '',
    startAdornment,
    type = 'text',
    error = false,
    ...props
}) => {
    // Validate that name and label are not empty as they are crucial for form fields
    if (!name) {
        console.error("FormField component: 'name' prop should not be empty.");
        return null;
    }
    if (!label) {
        console.error("FormField component: 'label' prop should not be empty.");
        return null;
    }
    return (
        <FormControl fullWidth={fullWidth} margin="normal">
            <Field
                as={TextField}
                fullWidth={fullWidth}
                id={name}
                name={name}
                label={label}
                type={type}
                error={error}
                InputProps={{
                    startAdornment: startAdornment ? <InputAdornment position="start">{startAdornment}</InputAdornment> : null,
                    endAdornment: endAdornment ? <InputAdornment position="end">{endAdornment}</InputAdornment> : null,
                }}
                {...props}
            />
            <ErrorMessage className='form-error' name={name} component="p" />
        </FormControl>
    );
}

type FormFieldProps = {
    endAdornment?: string;
    error?: boolean;
    fullWidth?: boolean,
    label: string;
    name: string;
    startAdornment?: string;
    type?: string;
    [key: string]: any;
}

export default FormField;
