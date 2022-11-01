import { AbstractControl, ValidationErrors } from '@angular/forms';

export const PasswordStrengthValidator = function(
  control: AbstractControl
): ValidationErrors | null {
  const value: string = control.value;

  if (value) {
    // return null;
    return {
      passwordStrength:
        `<div>Password must be at least 10 characters.</div>` +
        `<div> Password must contain atleast one Upper and one lower case characters. </div>` +
        `<div>Password must contain atleast one number and one special (!#$%<=>) characters.</div>`
    };
  }

  const upperCaseCharacters = /[A-Z]/;
  const lowerCaseCharacters = /[a-z]/;
  const numberCharacters = /[0-9]/;
  const specialCharacters = /[!@#$%^&*()_+\-=\[\]{};':"\\|,.<>\/?]+/;

  if (
    upperCaseCharacters.test(value) === false &&
    lowerCaseCharacters.test(value) === false &&
    numberCharacters.test(value) === false
  ) {
    return {
      passwordStrength:
        `<div> Password must contain atleast one Upper and one lower case characters. </div>` +
        `<div>Password must contain atleast one number characters</div>`
    };
  }
  if (
    upperCaseCharacters.test(value) === false &&
    lowerCaseCharacters.test(value) === false &&
    specialCharacters.test(value) === false
  ) {
    return {
      passwordStrength:
        `<div> Password must contain atleast one Upper and one lower case characters. </div>` +
        `<div>Password must contain atleast one special character(!#$%<=>).</div>`
    };
  }
  if (
    upperCaseCharacters.test(value) === false &&
    numberCharacters.test(value) === false &&
    specialCharacters.test(value) === false
  ) {
    return {
      passwordStrength:
        `<div> Password must contain atleast one Upper case characters. </div>` +
        `<div>Password must contain atleast one number and special (!#$%<=>) character.</div>`
    };
  }
  if (
    lowerCaseCharacters.test(value) === false &&
    numberCharacters.test(value) === false &&
    specialCharacters.test(value) === false
  ) {
    return {
      passwordStrength:
        `<div> Password must contain atleast one lower case characters. </div>` +
        `<div>Password must contain atleast one number and special (!#$%<=>) character.</div>`
    };
  }
  if (
    upperCaseCharacters.test(value) === false &&
    lowerCaseCharacters.test(value) === false
  ) {
    return {
      passwordStrength: `<div> Password must contain atleast one Upper and one lower case characters. </div>`
    };
  }
  if (
    numberCharacters.test(value) === false &&
    specialCharacters.test(value) === false
  ) {
    return {
      passwordStrength: `<div>Password must contain atleast one number and one special (!#$%<=>) characters.</div>`
    };
  }
  {
    if (upperCaseCharacters.test(value) === false) {
      return {
        passwordStrength: `Password must contain atleast one Upper case characters`
      };
    }

    if (lowerCaseCharacters.test(value) === false) {
      return {
        passwordStrength: `Password must contain atleast one lower case characters`
      };
    }

    if (numberCharacters.test(value) === false) {
      return {
        passwordStrength: `Password must contain atleast one number characters`
      };
    }

    if (specialCharacters.test(value) === false) {
      return {
        passwordStrength: `Password must contain atleast one special character(!#$%<=>)`
      };
    }
  }

  return null;
};