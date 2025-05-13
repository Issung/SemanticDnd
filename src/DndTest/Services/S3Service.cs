using Amazon.S3;
using Amazon.S3.Model;
using DndTest.Config;
using System.Net.Sockets;

namespace DndTest.Services;

public class S3Service(
    ILogger<S3Service> logger,
    DndSettings settings,
    IAmazonS3 client
)
{
    private readonly string bucketName = settings.BucketName;

    public async Task Put(string key, Stream data, string contentType)
    {
        var request = new PutObjectRequest
        {
            BucketName = bucketName,
            Key = key,
            InputStream = data,
            ContentType = contentType,
            AutoCloseStream = false,
        };

        await client.PutObjectAsync(request);

        logger.LogInformation("Uploaded {Key} to S3 bucket {BucketName}", key, bucketName);
    }

    public async Task<Stream?> Get(string key)
    {
        try
        {
            var response = await client.GetObjectAsync(bucketName, key);
            return response.ResponseStream;
        }
        catch (AmazonS3Exception e) when (e.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            logger.LogWarning("S3 object {Key} not found in bucket {BucketName}", key, bucketName);

            return null;
        }
    }

    public async Task<Uri> GetAccessUrl(string key)
    {
        var presignedUrlRequest = new GetPreSignedUrlRequest()
        {
            BucketName = bucketName,
            Key = key,
            Expires = DateTime.Now.AddDays(1),
            Protocol = Protocol.HTTP // TODO don't use HTTP outside 
        };

        var preSignedUrl = await client.GetPreSignedURLAsync(presignedUrlRequest);

        return new Uri(preSignedUrl);
    }
}
