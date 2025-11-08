using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECOMMERCE.SUBMISSION.HELPER;

public class EcommerceConstants
{
    //JWT
    public const string JWT_KEY = "JWT_KEY";
    public const string JWT_ISSUER = "JWT_ISSUER";
    public const string JWT_AUDIENCE = "JWT_AUDIENCE";

    //Redis
    public const string REDIS_ENDPOINTS = "REDIS_ENDPOINTS";
    public const string REDIS_USERNAME = "REDIS_USERNAME";
    public const string REDIS_PASSWORD = "REDIS_PASSWORD";

    //kafka
    public const string KAFKA_BOOTSTRAP = "KAFKA_BOOTSTRAP";
    public const string KAFKA_TOPIC = "KAFKA_TOPIC";
    public const string KAFKA_GROUP_ID = "KAFKA_GROUP_ID";

    //kafka topic
    public const string KAFKA_TOPIC_ORDER_PAYMENT_RESULT = "order.payment.result";
    public const string KAFKA_TOPIC_PROD_PROCESS = "prod.process";

    //Sendgrid
    public const string EMAIL_SENDGRID_KEY = "EMAIL_SENDGRID_KEY";

    //Error code
    public const string ERROR_NOT_FOUND = "ERROR_NOT_FOUND";
    public const string ERROR_INTERNAL_SERVER = "ERROR_INTERNAL_SERVER";
    public const string ERROR_DUPLICATE = "ERROR_DUPLICATE";
    public const string ERROR_INVALID_DATA = "ERROR_INVALID_DATA";
    public const string ERROR_INVALID_TOKEN = "ERROR_INVALID_TOKEN";
    public const string ERROR_INVALID_CREDENTIALS = "ERROR_INVALID_CREDENTIALS";
    public const string ERROR_VERIFY_CODE_EXPIRED = "ERROR_VERIFY_CODE_EXPIRED";
    public const string ERROR_VERIFY_CODE_INVALID = "ERROR_VERIFY_CODE_INVALID";
    public const string ERROR_VERIFY_CODE_TIME_LIMIT = "ERROR_VERIFY_CODE_TIME_LIMIT";
    public const string ERROR_BAD_REQUEST = "ERROR_BAD_REQUEST";

    //Message
    public const string MSG_INCORRECT_EMAIL_FORMAT = "Incorrect email format";
    public const string MSG_INCORRECT_PASSWORD_FORMAT = "Incorrect password format";

    //Regex
    public const string REGEX_PATTERN_EMAIL = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
    public const string REGEX_PATTERN_PASSWORD = @"^(?=.*[A-Za-z])(?=.*\d)(?=.*[^A-Za-z0-9]).{6,}$";
}
