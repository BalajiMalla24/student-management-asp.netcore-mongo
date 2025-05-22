    using Amazon.S3;
    using Amazon.S3.Model;
    using Amazon.S3.Transfer;
    using Microsoft.Extensions.Options;

    public class S3Service{


        private readonly IAmazonS3 _s3Client;
        private readonly AwsSettings _awsSettings;

    public S3Service(IOptions<AwsSettings> awsSettings)
        {
            _awsSettings = awsSettings.Value;
            _s3Client = new AmazonS3Client(_awsSettings.AccessKey, _awsSettings.SecretKey, Amazon.RegionEndpoint.GetBySystemName(_awsSettings.Region));
        }        



        public async Task<string?> UploadFileAsync(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            Console.WriteLine("Invalid file. File is null or empty.");
            return null;
        }

        try
        {
            var fileTransferUtility = new TransferUtility(_s3Client);
            using var stream = file.OpenReadStream();

            var key = Guid.NewGuid() + Path.GetExtension(file.FileName);

            await fileTransferUtility.UploadAsync(stream, _awsSettings.BucketName, key);

            var fileUrl = $"https://{_awsSettings.BucketName}.s3.{_awsSettings.Region}.amazonaws.com/{key}";

            Console.WriteLine($"File uploaded successfully to: {fileUrl}");

            return fileUrl;
        }
        catch (AmazonS3Exception s3Ex)
        {
            Console.WriteLine($"AWS S3 error: {s3Ex.Message}");
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"General error during file upload: {ex.Message}");
            return null;
        }
    }

 
    public string GenerateFileUrl(string key, int expireMinutes = 15)
    {
        var request = new GetPreSignedUrlRequest
        {
            BucketName = _awsSettings.BucketName,
            Key = key,
            Expires = DateTime.UtcNow.AddMinutes(expireMinutes)
        };

        string url = _s3Client.GetPreSignedURL(request);
        return url;
    }

    public async Task<(Stream?, string?, string?)> DownloadFileAsync(string key)
    {
        try
        {
            var response = await _s3Client.GetObjectAsync(_awsSettings.BucketName, key);
            return (response.ResponseStream, response.Headers["Content-Type"], response.Metadata["x-amz-meta-originalfilename"] ?? key);
        }
        catch (AmazonS3Exception ex)
        {
            Console.WriteLine($"S3 error during download: {ex.Message}");
            return (null, null, null);
        }
    }


    }