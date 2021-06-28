package be.kdg.stemtest.DI.modules

import android.annotation.SuppressLint
import android.app.Application
import com.google.gson.GsonBuilder
import com.microsoft.signalr.HubConnection
import com.microsoft.signalr.HubConnectionBuilder
import dagger.Module
import dagger.Provides
import okhttp3.Cache
import okhttp3.OkHttpClient
import okhttp3.logging.HttpLoggingInterceptor
import retrofit2.Retrofit
import retrofit2.adapter.rxjava2.RxJava2CallAdapterFactory
import retrofit2.converter.gson.GsonConverterFactory
import java.io.File
import java.security.SecureRandom
import java.security.cert.X509Certificate
import java.util.concurrent.TimeUnit
import javax.net.ssl.HostnameVerifier
import javax.net.ssl.SSLContext
import javax.net.ssl.SSLSocketFactory
import javax.net.ssl.TrustManager
import javax.net.ssl.X509TrustManager

private val ip="https://www.daemonsstemtest.be/api/"

@Module
class NetworkModule {
    @Provides
     fun getRetrofit(application: Application): Retrofit{
        return Retrofit.Builder()
            //zorgt voor translation tussen RXJava observables en Retrofit Call
            .addCallAdapterFactory(RxJava2CallAdapterFactory.create())
            .client(getClient(application))
            .addConverterFactory(GsonConverterFactory.create(GsonBuilder().setLenient().create()))
            .baseUrl(ip)
            .build()
    }

    private fun getClient(application: Application): OkHttpClient {

        val trustAllCerts: Array<TrustManager> = arrayOf(
            object : X509TrustManager {
                @SuppressLint("TrustAllX509TrustManager")
                override fun checkClientTrusted(chain: Array<X509Certificate>, authType: String) {
                }

                @SuppressLint("TrustAllX509TrustManager")
                override fun checkServerTrusted(chain: Array<X509Certificate>, authType: String) {
                }

                override fun getAcceptedIssuers(): Array<X509Certificate> {
                    return arrayOf()
                }
            }
        )
        // Install the all-trusting trust manager
        val sslContext = SSLContext.getInstance("SSL")
        sslContext.init(null, trustAllCerts, SecureRandom())
        // Create an ssl socket factory with our all-trusting manager
        val sslSocketFactory: SSLSocketFactory = sslContext.socketFactory

        val cache = Cache(
            File(application.cacheDir, "http"),
            2000000
        )
        val logger = HttpLoggingInterceptor()
        logger.level=HttpLoggingInterceptor.Level.BODY

        return OkHttpClient.Builder()
            .connectTimeout(2, TimeUnit.SECONDS)
            .readTimeout(2, TimeUnit.SECONDS)
            .writeTimeout(2, TimeUnit.SECONDS)
            .sslSocketFactory(sslSocketFactory, trustAllCerts[0] as X509TrustManager)
            .hostnameVerifier(HostnameVerifier { _, _ -> true })
            .addInterceptor(logger)
            .cache(cache)
            .build()
    }

    @Provides
    fun getHubConnection(application: Application): HubConnection {
         val conn=HubConnectionBuilder
           .create(ip +"sessionHub")
             .shouldSkipNegotiate(true)
           .build()

       val httpClientField = conn.javaClass.getDeclaredField("httpClient")
        httpClientField.isAccessible = true
        val httpClientFieldValue = httpClientField.get(conn)
        val clientField = httpClientFieldValue.javaClass.getDeclaredField("client")
        clientField.isAccessible = true
        clientField.set(httpClientFieldValue, getClient(application))

        return conn
    }

}