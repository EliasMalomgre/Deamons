package be.kdg.stemtest.view.fragments

import android.content.Intent
import android.net.Uri
import android.os.Bundle
import androidx.fragment.app.Fragment
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import be.kdg.stemtest.R
import life.sabujak.roundedbutton.RoundedButton


class PrivacyStatement : Fragment() {

    private lateinit var button: RoundedButton


    override fun onCreateView(
        inflater: LayoutInflater, container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View? {
        return inflater.inflate(R.layout.fragment_privacy_statement, container, false)
    }

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)
        initialiseViews(view)
        addEvenHandlers()

    }

    private fun initialiseViews(view: View){
        button = view.findViewById(R.id.btnPrivacyWeb)
    }

    private fun addEvenHandlers(){
        button.setOnClickListener{
            v->
            val intent = Intent(Intent.ACTION_VIEW, Uri.parse("https://www.daemonsstemtest.be/Home/Privacy"));
            startActivity(intent);
        }
    }
}
