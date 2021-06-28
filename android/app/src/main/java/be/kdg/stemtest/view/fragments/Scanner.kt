package be.kdg.stemtest.view.fragments


import android.Manifest
import android.content.pm.PackageManager
import android.os.Bundle
import androidx.fragment.app.Fragment
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import androidx.core.app.ActivityCompat
import androidx.core.content.ContextCompat
import androidx.core.os.bundleOf
import androidx.navigation.Navigation
import be.kdg.stemtest.R
import com.budiyev.android.codescanner.CodeScanner
import com.budiyev.android.codescanner.CodeScannerView
import com.budiyev.android.codescanner.DecodeCallback

const val MY_CAMERA_PERMISSION_REQUEST = 0

class Scanner : Fragment() {



    private lateinit var codeScanner: CodeScanner
    private lateinit var codeScannerView: CodeScannerView
    var code = "0"

    override fun onCreateView(inflater: LayoutInflater, container: ViewGroup?,
                              savedInstanceState: Bundle?): View? {
        return inflater.inflate(R.layout.scanner_fragment, container, false)

    }

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)
        initialiseViews(view)
        setScannerDecoder(view)
    }

    private fun setScannerDecoder(view: View) {
        codeScanner.decodeCallback = object: DecodeCallback {
            override fun onDecoded(result: com.google.zxing.Result) {
                code = result.text.toString().substringAfter("=")
                if (!code.equals(0)){
                    navigateOnMain(code,view)
                }
            }
        }
    }


    fun initialiseViews(view: View){
        codeScannerView = view.findViewById(R.id.scannerView)
        codeScanner = CodeScanner(view.context,codeScannerView)
    }

    override fun onResume() {
        super.onResume()
        requestForCamera()
        codeScanner.startPreview()
    }


    fun navigateOnMain(string: String,view: View){
        val bundle = bundleOf("code" to string.toInt())

        Navigation.findNavController(view).navigate(
            R.id.action_scanner_to_connect,
            bundle)
    }

    fun requestForCamera(){

        if (ContextCompat.checkSelfPermission(
                requireContext(),
                Manifest.permission.CAMERA)
            != PackageManager.PERMISSION_GRANTED) {

            if (ActivityCompat.shouldShowRequestPermissionRationale(
                    this.requireActivity(),
                    Manifest.permission.CAMERA)) {
            } else {
                ActivityCompat.requestPermissions(this.requireActivity(),
                    arrayOf(Manifest.permission.CAMERA),
                    MY_CAMERA_PERMISSION_REQUEST
                )
            }
        }
    }


}
